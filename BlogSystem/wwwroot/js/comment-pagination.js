/**
 * This code handles the pagination of entry comments on the Home/EntryComments view.
 */
(function ($) {
    var pagify = {
        items: {},
        container: null,
        totalPages: 1,
        perPage: 5,
        currentPage: 0,
        createNavigation: function () {

            this.totalPages = Math.ceil(this.items.length / this.perPage);

            $('.comment-pagination', this.container.parent()).remove();
            var pagination = $('<ul class="pagination comment-pagination"></ul>')
                .append('<li><a id="first-cmt-page" class="page" data-page="1" title="Go to first comment.">&laquo;</a></li>');

            for (var i = 0; i < this.totalPages; i++) {
                var className = "page";

                if (!i)
                    className = "page current";

                var markup = '<li><a class="' + className + '" data-page="' + (i + 1) + '">' + (i + 1) + "</a></li>";

                pagination.append(markup);
            }

            pagination.append('<li><a id="last-cmt-page" class="page" data-page="' + this.totalPages + '" title="Go to last comment.">&raquo;</a></li>');

            this.container.after(pagination);

            var that = this;
            $("body").off("click", ".nav");
            this.navigator = $("body").on("click", ".nav", function () {
                var el = $(this);
                that.navigate(el.data("next"));
            });

            $("body").off("click", ".page");
            this.pageNavigator = $("body").on("click", ".page", function () {
                var el = $(this);
                that.goToPage(el.data("page"));
            });
        },

        navigate: function (next) {
            if (isNaN(next) || next === undefined) {
                next = true;
            }
            if (next) {
                this.currentPage++;

                if (this.currentPage > (this.totalPages - 1))
                    this.currentPage = (this.totalPages - 1);
            }
            else {
                this.currentPage--;

                if (this.currentPage < 0)
                    this.currentPage = 0;
            }
            this.showItems();
        },

        updateNavigation: function () {

            var pages = $(".comment-pagination .page");
            pages.removeClass("current");

            $('.comment-pagination .page[data-page="' + (this.currentPage + 1) + '"]').addClass("current");

            if ($("#first-cmt-page").hasClass("current"))
                $("#first-cmt-page").removeClass("current");

            if ($("#last-cmt-page").hasClass("current"))
                $("#last-cmt-page").removeClass("current");
        },

        goToPage: function (page) {
            this.currentPage = page - 1;
            this.showItems();
        },

        showItems: function () {
            this.items.hide();
            var base = this.perPage * this.currentPage;
            this.items.slice(base, base + this.perPage).show();

            this.updateNavigation();
        },

        init: function (container, items, perPage) {
            this.container = container;
            this.currentPage = 0;
            this.totalPages = 1;
            this.perPage = perPage;
            this.items = items;
            this.createNavigation();
            this.showItems();
        }
    };

    // Stuff it all into a jQuery method:
    $.fn.pagify = function (perPage, itemSelector) {
        var element = $(this);
        var items = $(itemSelector, element);

        // Default perPage to 5:
        if (isNaN(perPage) || perPage === undefined) {
            perPage = 5;
        }

        // Don't fire if fewer items than perPage:
        if (items.length <= perPage) {
            return true;
        }

        pagify.init(element, items, perPage);
    };
})(jQuery);

// Items to display per page:
$(".comment-container").pagify(20, ".single-comment");