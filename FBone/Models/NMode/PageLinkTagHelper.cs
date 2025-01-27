using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using FBone.Models.NMode;
using System;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace FBone.Models.NMode
{
    public class PageLinkTagHelper : TagHelper
    {
        IUrlHelperFactory urlHelperFactory;
        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;
        public PageViewModel PageModel { get; set; }
        public string PageController { get; set; } = "";
        public string PageAction { get; set; } = "";
        public int PageAreaId { get; set; }
        public int PageLcnId { get; set; }
        public int PageSize { get; set; }
        public string PageSearch { get; set; }
        public bool PageCalculated { get; set; }
        public bool Snapshot { get; set; }
        public DateTime SelectedDate { get; set; }
        public bool ValidateTags { get; set; }
        public bool ResultDetails { get; set; }
        public bool HideDetails { get; set; }
        public bool HideInActiveRecords { get; set; }
        public bool Hide100PercentResults { get;set; }
        public SortState SortOrder { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (PageModel == null) throw new Exception("PageModel is not set");

            if (PageModel.PageSize == 0 || PageModel.PageSize >= PageModel.RecordCount || PageModel.RecordCount == 0) return;

            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "div";

            // ul list
            TagBuilder tag = new TagBuilder("ul");
            tag.AddCssClass("pagination");

            // current page link
            TagBuilder currentItem = CreateTag(urlHelper, PageModel.PageNumber);

            //form link to first page
            if (PageModel.PageNumber > 1 && PageModel.TotalPages > 1)
            {
                TagBuilder startItem = CreateTag(urlHelper, 1);
                tag.InnerHtml.AppendHtml(startItem);
            }
            if (PageModel.PageNumber > 3 && PageModel.TotalPages >= 4)
            {
                TagBuilder elipsisItem = CreateTag(urlHelper, PageModel.PageNumber - 2, PageModel.TotalPages > 4);
                tag.InnerHtml.AppendHtml(elipsisItem);
            }
            // form link to previous page if it exists
            if (PageModel.HasPreviousPage && PageModel.PageNumber != 2)
            {
                TagBuilder prevItem = CreateTag(urlHelper, PageModel.PageNumber - 1);
                tag.InnerHtml.AppendHtml(prevItem);
            }

            tag.InnerHtml.AppendHtml(currentItem);
            // создаем ссылку на следующую страницу, если она есть
            if (PageModel.HasNextPage && PageModel.PageNumber + 1 < PageModel.TotalPages)
            {
                TagBuilder nextItem = CreateTag(urlHelper, PageModel.PageNumber + 1);
                tag.InnerHtml.AppendHtml(nextItem);
            }
            if (PageModel.TotalPages - PageModel.PageNumber > 2)
            {
                TagBuilder elipsisItem = CreateTag(urlHelper, PageModel.PageNumber + 2, PageModel.TotalPages > 4);
                tag.InnerHtml.AppendHtml(elipsisItem);
            }
            if (PageModel.TotalPages != PageModel.PageNumber)
            {
                TagBuilder endItem = CreateTag(urlHelper, PageModel.TotalPages);
                tag.InnerHtml.AppendHtml(endItem);
            }
            output.Content.AppendHtml(tag);
        }

        TagBuilder CreateTag(IUrlHelper urlHelper, int pageNumber = 1, bool elipsis = false)
        {
            TagBuilder item = new TagBuilder("li");
            TagBuilder link = new TagBuilder("a");
            //if (!elipsis)
            //{
            if (pageNumber == PageModel?.PageNumber)
            {
                item.AddCssClass("active");
            }
            else
            {
                link.Attributes["href"] = urlHelper.Action(PageAction,
                    new
                    {
                        page = pageNumber,
                        AreaID = PageAreaId,
                        LcnId = PageLcnId,
                        search = PageSearch,
                        pagesize = PageSize,
                        calculated = PageCalculated,
                        snapshot = Snapshot,
                        selectedDate = SelectedDate,
                        validateTags = ValidateTags,
                        resultDetails = ResultDetails,
                        hideDetails = HideDetails,
                        sortOrder = SortOrder,
                        hideInActiveRecords = HideInActiveRecords,
                        hide100PercentResults= Hide100PercentResults
                    });
            }
            //}

            item.AddCssClass("page-item");
            link.AddCssClass("page-link");
            if (elipsis)
                link.InnerHtml.Append("...");
            else
                link.InnerHtml.Append(pageNumber.ToString());
            item.InnerHtml.AppendHtml(link);
            return item;
        }
    }
}
