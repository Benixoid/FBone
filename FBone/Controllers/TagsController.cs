using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FBone.Database;
using FBone.Database.Entities;
using FBone.Service.Authorize;
using FBone.Models.Tags;
using Inventory.Models.NetworkDeviceModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Routing;

namespace FBone.Controllers
{
    [Authorize("USER_SHIFT")]
    public class TagsController : Controller
    {
        private readonly DataManager _dataManager;

        public TagsController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public IActionResult Index(int page, string SearchString)
        {
            if (page == 0 )
                page = 1;
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var list = _dataManager.Tag.GetTags(user);
            if (!SearchString.IsNullOrEmpty())
                list = list
                    .Where(i => 
                    i.Tagnumber.Contains(SearchString) 
                    || i.TagnumberByp.Contains(SearchString)
                    || i.Equipment.Contains(SearchString)
                    || i.Type.Contains(SearchString)
                    || i.Service.Contains(SearchString)
                    || i.Unit.Contains(SearchString)
                    || i.Area.Name_EN.Contains(SearchString)
                    || i.Area.Name_RU.Contains(SearchString)
                    || i.Area.Name_KK.Contains(SearchString)
                    || i.Area.Facility.Name.Contains(SearchString)
                    || i.Device.Name.Contains(SearchString)
                    );
            var model = new TagsModel
            {                
                Page = page,
                SearchString = SearchString
            };
            PaginatedListTags<Tag> repList = PaginatedListTags<Tag>.CreateAsync(list.OrderBy(i => i.Tagnumber), model.Page, 30);
            if (repList != null)
            {
                model.HasNextPage = repList.HasNextPage;
                model.HasPreviousPage = repList.HasPreviousPage;
                model.TotalPages = repList.TotalPages;
                model.Page = repList.PageIndex;
                model.TotalEntities = repList.TotalEntities;
                model.Tags = repList;
            }
            return View(model);
        }

        public IActionResult CreateTag(string SearchString)
        {
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var model = new TagEditModel
            {
                Tag = new(),
                SearchString = SearchString,
                Devices = new SelectList(_dataManager.Device.GetDevices() , "Id", "Name"),
                Areas = _dataManager.tArea.getAreaSelectList(user)
            };
            return View("TagDetails", model);
        }

        public IActionResult EditTag(int id, string SearchString )
        {
            if (id == 0)
            {
                return NotFound();
            }

            var tag = _dataManager.Tag.GetTagById(id);
            if (tag == null)
            {
                return NotFound();
            }
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var model = new TagEditModel
            {
                Tag = tag,
                SearchString = SearchString,
                Devices = new SelectList(_dataManager.Device.GetDevices(), "Id", "Name"),
                Areas = _dataManager.tArea.getAreaSelectList(user)
            };           
            return View("TagDetails", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveTag(Tag tag, string SearchString)
        {
            if(!tag.Tagnumber.IsNullOrEmpty())
            {
                var tagDB = _dataManager.Tag.GetTagByName(tag.Tagnumber);
                if (tagDB != null && (tag.Id == 0 || tag.Id != tagDB.Id))
                {
                    ModelState.AddModelError("Tag.Tagnumber", "Tagnumber exist!");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dataManager.Tag.SaveTag(tag);
                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return RedirectToAction(nameof(Index), new { page = 1, SearchString = SearchString });
            }
            var user = _dataManager.tUser.GetUserByCAI(User.Identity.Name);
            var model = new TagEditModel
            {
                Tag = tag,
                Devices = new SelectList(_dataManager.Device.GetDevices(), "Id", "Name"),
                Areas = _dataManager.tArea.getAreaSelectList(user)
            };
            return View("TagDetails",model);
        }       
    }
}
