// Scripts/Selections.js
// Auteur original : Nicolas Chourot

$(document).ready(initUI);

function initUI() {
    sortAllSelect();
    deSelectAll($('body'));


    $('.UnselectedItems').change(function (e) {
        let parent = $(this).parent();
        parent.find('.UnselectedItems option:selected').each(function () {
            parent.find(".SelectedItems option:selected").prop("selected", false);
            parent.find('.AddSelection').show();
            parent.find('.RemoveSelection').hide();
            parent.find('.UnselectAll').show();
        });
        e.preventDefault();
    });


    $('.SelectedItems').change(function (e) {
        let parent = $(this).parent();
        parent.find('option:selected').each(function () {
            parent.find(".UnselectedItems option:selected").prop("selected", false);
            parent.find('.AddSelection').hide();
            parent.find('.RemoveSelection').show();
            parent.find('.UnselectAll').show();
        });
        e.preventDefault();
    });


    $(document).on('submit', 'form', function () {
        $('.SelectedItems option').prop('selected', true);
    });

    $(".AddSelection").on('click', function () {
        let parent = $(this).parent().parent();
        parent.find('.UnselectedItems').first()
            .find('option:selected').each(function () {
                $(this).remove();
                parent.find('.SelectedItems').first().append($(this));
                sortSelect(parent.find(".SelectedItems").first());
                scrollTo(parent.find(".SelectedItems").first(), $(this).offset().top);
                parent.find(".SelectedItems").focus();
            });
        parent.find('.AddSelection').hide();
        parent.find('.RemoveSelection').show();
        parent.find('.UnselectAll').show();
    });


    $(".RemoveSelection").on('click', function () {
        let parent = $(this).parent().parent();
        parent.find('.SelectedItems').first()
            .find('option:selected').each(function () {
                $(this).remove();
                parent.find('.UnselectedItems').first().append($(this));
                sortSelect(parent.find(".UnselectedItems").first());
                scrollTo(parent.find(".UnselectedItems").first(), $(this).offset().top);
                parent.find(".UnselectedItems").focus();
            });
        parent.find('.AddSelection').show();
        parent.find('.RemoveSelection').hide();
        parent.find('.UnselectAll').show();
    });


    $(".UnselectAll").on('click', function () {
        let parent = $(this).parent().parent();
        deSelectAll(parent);
    });
}

function deSelectAll(parent) {
    parent.find('.AddSelection').hide();
    parent.find('.RemoveSelection').hide();
    parent.find('.UnselectAll').hide();
    parent.find('.SelectedItems option').prop('selected', false);
    parent.find('.UnselectedItems option').prop('selected', false);
}


function sortSelect(select) {
    select.each(function () {
        let sel = $(this);
        sel.html(sel.find('option').sort(function (a, b) {
            return $(a).text() < $(b).text() ? -1 : 1;
        }));
    });
}

function sortAllSelect() {
    $('select').each(function () {
        let sel = $(this);
        sel.html(sel.find('option').sort(function (a, b) {
            return $(a).text() < $(b).text() ? -1 : 1;
        }));
    });
}

function scrollTo(selectObj, optionTop) {
    var selectTop = selectObj.offset().top;
    selectObj.scrollTop(selectObj.scrollTop() + (optionTop - selectTop));
}


function RestoreDetailsState() {
    $("details").off();
    $("details").on('toggle', function () {
        let details_dom = $(this)[0];
        if (details_dom != undefined) {
            localStorage.setItem(details_dom.id, details_dom.open);
        }
    });

    for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i);
        if (key.indexOf("details") > -1) {
            let details_dom = $("#" + key)[0];
            if (details_dom != undefined)
                details_dom.open = localStorage.getItem(key) == "true";
        }
    }
}