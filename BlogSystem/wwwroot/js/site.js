/*
    Created: 2020
    Author: Annice Strömberg
*/

$(document).ready(function () {

    // Apply fade out effect on feedback messages:
    $("#feedback").fadeOut(3000);

    /**
    * Handle client side validation for the entry submit (create and edit view).
    */
    $("#entryForm").submit(function (e) {

        var isCategoryChecked = false;

        $('#entryForm input[type="checkbox"]').each(function () {
            if ($(this).is(":checked")) {
                isCategoryChecked = true;
            }
        });

        if (isCategoryChecked == false) {

            if ($('#category-input-error').length == 0) {

                $(".category-feedback")
                    .append('<span id="category-input-error" class="text-danger">Please select a category.</span>');

                $(".category-validation").addClass('red-border-category').removeClass('category-validation');
            }
            e.preventDefault();
            return false;
        }
        if (!$("#tinymce").val()) {

            if ($('#entry-input-error').length == 0) {

                $(".entry-feedback")
                    .append('<span id="entry-input-error" class="text-danger">Please write an entry.</span>');

                $(".entry-validation").addClass('red-border-entry').removeClass('entry-validation');
            }
            e.preventDefault();
            return false;
        }
    });

});

/**
 * Global method to select a checkbox by its ID and in turn trigger a
 * multi-select function of other checkboxes called by their input names.
 * @param {any} id
 * @param {any} element
 */
multiSelect = function selectAll(id, element) {

    if ($("#" + id).is(":checked")) {
        $(element).each(function () {
            $(this).prop("checked", true);
        });
    }
    else {
        $(element).each(function () {
            $(this).prop("checked", false);
        });
    }
}

/**
 * Handle the category dropdown list called by its ID name from the entry views:
 */
if (window.location.pathname.indexOf("/Entry/Create") > -1 || window.location.pathname.indexOf("/Entry/Edit") > -1) {

    var checkList = document.getElementById('categories');

    checkList.getElementsByClassName('anchor')[0].onclick = function (e) {

        if (checkList.classList.contains('visible'))
            checkList.classList.remove('visible');
        else
            checkList.classList.add('visible');
    }
}
