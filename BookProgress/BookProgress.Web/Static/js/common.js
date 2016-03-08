var KEY = {
    ENTER: 13
}

$(function () {
    initContextMenuEvents();
    initEditableDropdown();
    initSearchFieldEvents();
    initConfirmDialogsEvents();
    initDebugger();
    tabs.initTabsEvents();
    inputs.initPicklistDropdowns();
    tables.initPaging();
    structure.initStructure();
    tables.initSorting();

    $(document).ajaxError(function (e, response) {
        ajax.toggleLoader();
        if (response.responseText != null && response.responseText.length > 0) {
            $(response.responseText).modal();
        } else {
            alert("No response from server");
        }
    });
    $(document).ajaxSend(function () {
        ajax.toggleLoader();
    });
    $(document).ajaxSuccess(function () {
        ajax.toggleLoader();
    });
});

var modals = {
    showPopover: function (element, htmlContent) {

        var $element = $(element);
        var $popover = null;
        if ($element.data('bs.popover') == null) {
            var popoverPlacement = $element.data('popover-content-placement');
            $element.popover({
                content: htmlContent,
                html: true,
                placement: popoverPlacement != null ? popoverPlacement : 'right',
                trigger: 'click',
                animation: false,
                selector: '[rel="popover"]',
                template: '<div class="popover" role="tooltip"><div class="arrow"></div><div class="popover-content"></div></div></div>',
                container: document.body
            });
            $popover = $element.data('bs.popover').tip();
            $element.on('shown.bs.popover', function () {
                // POPOVER CONTENT CAN BE UPDATED AND HEIGHT WILL BE CHANGED, SO FIX THE POSITION OF ARROW
                var $arrow = $element.data('bs.popover').$arrow;
                var topPosition = $arrow.position().top;
                $arrow.css("top", topPosition);
            });
            $popover.on("click", ".close, .button-close", function () {
                $element.popover("hide");
                return false;
            });
        } else {
            $popover = $element.data('bs.popover').tip();
            $element.data('bs.popover').options.content = htmlContent;
            $popover.find(".popover-content").html(htmlContent);
        }
        
        $element.popover('show');
        return $popover;
    }
}

var ajax = {

    submitSubform: function (element, successCallback, aditionalForm) {
        var $form = $(element).closest(".subform");
        if (aditionalForm != null)
            $form = $form.add("<div>" + aditionalForm + "</div>");
        var data = $form.find(":input").serializeArray();
        $.post($form.data("subform-action"), data, function (response) {
            successCallback(response);
        });
    },

    onFormSubmitResponse: function (element, successCallback) {
        var $form = $(element).find("form")
        $form.on("submit", function () {
            var data = $(this).serializeArray();
            data.push({
                name: "modalFormSubmit",
                value: true
            });
            $.post(this.action, data, function (response) {
                successCallback != null && successCallback(response, this);
            });
            return false;
        })
    },

    toggleLoader: function () {
        $('#ajax-loader').toggle(0);
    }
}

var tabs = {
    initTabsEvents: function () {
        $(document.body).on("click", ".tab-trigger", function () {
            var $trigger = $(this);
            var tabBodyId = $trigger.data("tab-body-id");
            var $tabBody = $("#" + tabBodyId);
            $tabBody.closest(".tabs").children(".tab-body").hide();
            $tabBody.show();

            $trigger.parent().children(".selected").removeClass("selected");
            $trigger.addClass("selected");

            var callbackName = $trigger.data("tab-callback");
            if (window[callbackName] != null)
            {
                $.proxy(window[callbackName], $trigger.get(0))();
            }

        });
    }
}

var inputs = {
    initPicklistDropdowns: function () {
        $(document.body).on("change", "select.picklist-dropdown", function () {
            $(this).parent().find("input:hidden").val($(this).find("option:selected").text());
        })
    }
}

var tables = {
    paging: {
        showItemsRange: function ($pagingContainer, from, to) {
            $trs = $pagingContainer.find("table tbody tr.top-level");
            $trs.hide();
            $trs.slice(from, to).show();
        },
        switchPage: function ($pagingContainer, pageNumber) {
            if ($pagingContainer.length > 0) {
                var itemsPerPage = parseInt($pagingContainer.find('.paging-toolbar').data('items-per-page'));
                var $ul = $pagingContainer.find('ul.pagination');

                $ul.find('li:not(.always-show)').hide();

                var from = pageNumber >= 10 ? (((pageNumber - (pageNumber % 10)) / 10) * 10) : 1;
                for (i = from; i < from + 10; i++) {
                    $ul.find('li[data-page="' + i + '"]').show();
                }

                $ul.find('li.active').removeClass('active');
                $ul.find('li[data-page="' + pageNumber + '"]').addClass('active');
                $ul.data('active-page', pageNumber);

                tables.paging.showItemsRange($pagingContainer, pageNumber * itemsPerPage - itemsPerPage, pageNumber * itemsPerPage);
                urlHash.setParametr('page', pageNumber);
            }
        }
    },
    initPaging: function () {
        $('ul.pagination li:not(.always-show)').hide();
        var firstPage = urlHash.getParametr('page');
        if (firstPage == null || $('.paging-container ul.pagination li[data-page="' + firstPage + '"]').length == 0) {
            firstPage = 1;
        }

        tables.paging.switchPage($('.paging-container'), firstPage);

        $('.paging-container').on("click", "ul.pagination li.previous", function () {
            var $ul = $(this).closest('ul');
            var activePage = parseInt($ul.data('active-page'));
            
            if (activePage > 1) { 
                tables.paging.switchPage($ul.closest('.paging-container'), activePage - 1);
            }
        });
        $('.paging-container').on("click", "ul.pagination li.next", function () {
            var $ul = $(this).closest('ul');
            var activePage = parseInt($ul.data('active-page'));
            var liCount = $ul.find('li.page').length;

            if (activePage != liCount) {
                tables.paging.switchPage($ul.closest('.paging-container'), activePage + 1);
            }
        });
        $('.paging-container').on("click", "ul.pagination li:not(.previous, .next)", function () {
            var $li = $(this);
            tables.paging.switchPage($li.closest('.paging-container'), parseInt($li.data('page')));
        });

        $('.paging-container').removeClass('hidden');
    },
    sorting: {
        sortedValueSelector: null,
        comparator: function (columnIndex) {
            return function (rowA, rowB) {
                var valueA = tables.sorting.cellValue(rowA, columnIndex);
                var valueB = tables.sorting.cellValue(rowB, columnIndex);
                if ($.isNumeric(valueA) && $.isNumeric(valueB)) {
                    return valueA - valueB;
                } else if (valueA == null && valueB == null) {
                    return 0;
                } else if (valueA == null && valueB != null) {
                    return 1;
                } else if (valueA != null && valueB == null) {
                    return -1;
                } else {
                    return valueA.localeCompare(valueB);
                }
            }
        },
        cellValue: function (row, columnIndex) {
            var val = "";
            if (tables.sorting.sortedValueSelector) {
                val = $.trim($(row).children('td').eq(columnIndex).find(tables.sorting.sortedValueSelector).text());
            } else {
                val = $.trim($(row).children('td').eq(columnIndex).text());
            }
            if (val.length == 0) {
                val = null;
            }
            return val;
        }
    },
    initSorting: function () {
        $('table.table').on('click', 'th.sort', function () {
            var $this = $(this);
            var $table = $this.closest('table');
            var $rows = $table.find('tbody tr');

            $this.closest('thead').find('tr th i.sort').remove();

            tables.sorting.sortedValueSelector = $this.data("sorted-value-selector");

            if ($table.find('tr.top-level:first').length == 1) {
                $rows.filter('tr:not(.top-level)').remove();
                $rows = $rows.filter('tr.top-level');
            }
            ajax.toggleLoader();
            $rows = $rows.toArray().sort(tables.sorting.comparator($this.index()));
            
            $this.data('asc', !$this.data('asc'));
            if (!$this.data('asc')) {
                $rows = $rows.reverse();
            }
            $table.append($rows);
            $this.append('<i class="sort glyphicon glyphicon-' + ($this.data('asc') ? 'chevron-down' : 'chevron-up') + '"></i>');
            
            //show right rows
            var $pagingContainder = $table.closest('.paging-container');
            tables.paging.switchPage($pagingContainder, parseInt($pagingContainder.find('.pagination').data('active-page')));
            ajax.toggleLoader();
        });
    }
}

var urlHash = {
    getParamsAsObject: function () {
        var hashString = window.location.hash.substr(1)
        if (hashString == "") {
            return {};
        }
        var keyPairArray = hashString.split('&');
        var keyPairObj = {}
        for (i = 0; i < keyPairArray.length; i++) {
            keyPairArray[i].replace(/\+/g, ' ');
            nameval = keyPairArray[i].split('=', 2);
            keyPairObj[decodeURIComponent(nameval[0])] = decodeURIComponent(nameval[1]);
        }
        return keyPairObj;
    },
    getParametr: function (name) {
        return urlHash.getParamsAsObject()[name];
    },
    setParametr: function (name, value) {
        var params = urlHash.getParamsAsObject();
        params[name] = value;

        window.location.hash = "#" + $.param(params);
    }
}

var structure = {
    initStructure: function () {
        $('.structure-box').on('click', '.customer-item', function (e) { // select item
            var $box = $(this).closest('.structure-box');
            $box.find('.customer-item').removeClass('selected');
            $(this).addClass('selected');
            e.stopImmediatePropagation(); // prevent parent <li>s to be selected as well
        });
        $('.structure-box').on('click', '.add-item', function () { // redirect to form for adding a child item
            var formUrl = $(this).closest('.structure-box').find('.customer-item.selected').data('child-form-url');
            if (formUrl)
                window.location.href = formUrl;
        });
    }
}


function initContextMenuEvents() {

    var contextMenuClicked = false;
    var contextMenuVisible = false;
    var selectedRow = null;

    var hideContextMenu = function () {
        $(document.body).find(".context-menu.visible").remove();
        $(selectedRow).removeClass("selected");
        contextMenuVisible = false;
    };

    $(document.body).on("click", ".table .context-menu-trigger", function (e) {

        if (contextMenuVisible) {
            hideContextMenu();
        }

        selectedRow = $(this).closest("tr");
        $(selectedRow).addClass("selected");

        var $contextMenu = $(selectedRow).find(".context-menu");
        if ($contextMenu.size() == 0)
            return;

        $cloned = $contextMenu.clone();
        $cloned.css({ 'top': e.pageY, 'left': e.pageX })
            .addClass("visible")
            .appendTo(document.body)
            .show();

        $cloned.click(function () {
            contextMenuClicked = true;
        });

        contextMenuVisible = true;
        return false;

    });

    $(window).on("click", function () {
        if (!contextMenuClicked && contextMenuVisible) {
            hideContextMenu();
        }
        contextMenuClicked = false;
    });
};

function initSearchFieldEvents() {

    $(document.body).on('click', '.modal .close, .modal-close, .modal-backdrop', function () {
        $(this).parents(".modal").modal('hide');
    });

    var $modal;
    var $currentInutGroup;
    var onSelectedCallbackName = null;

    $(document.body).on('click', '.modal-trigger', function () {

        $currentInutGroup = $(this).closest("div.input-group");
        onSelectedCallbackName = $(this).data("on-selected-callback-name");

        $.get($(this).data('modal-content-url'), { selectedId: $currentInutGroup.children("input[type='hidden']").val() }, function(data) {
            $modal = $(data);
            $modal.modal();
            $modal.filter('.modal').on('hidden.bs.modal', function() {
                $modal.remove();
            });
        });

    });

    $(document.body).on('click', '.remove-searched-data-trigger', function () {

        $currentInutGroup = $(this).closest("div.input-group");
        $currentInutGroup.children("input[type='hidden']").val("");
        $currentInutGroup.children("input[type='text']").val("");
    });

    $(document.body).on('click', '.apply-filter-trigger', function () {
        var $form = $(this).closest("form");
        var formData = $form.serializeArray();
        $.ajax({
            url: $form.attr("action"),
            data: formData,
            type: "GET",
            dataType: "html",
            success: function(data) {
                $modal.find('.modal-body').html(data);
            }
        });
        return false;
    });

    $(document.body).on('keydown', '.filter-form input[type="text"]', function (e) {
        if (e.which == KEY.ENTER) {
            e.stopImmediatePropagation();
            e.preventDefault();
            $(this).closest("form").find('.apply-filter-trigger').click();
            return false;
        }
    });

    $(document.body).on('click', '.row-select-trigger', function() {
        var $target = $(this);
        $currentInutGroup.children("input[type='hidden']").val($target.data("id"));
        $currentInutGroup.children("input[type='text']").val($target.data("label"));
        $modal.modal('hide');
        if (onSelectedCallbackName) {
            var callback = window[onSelectedCallbackName];
            callback($target);
        }

    });

};

function initEditableDropdown() {

    var $visibleDropDownForInputTrigger = null;

    $(document.body).on("click", ".dropdown-for-input-toggle", function () {
        $(this).closest(".input-group").toggleClass("open");
        $(this).closest(".input-group-btn").toggleClass("open");
        $visibleDropDownForInputTrigger = $(this);
    });

    $(document.body).on("click", ".dropdown-for-input a", function () {
        var $link = $(this);
        $link.closest(".input-group").toggleClass("open");
        $link.closest(".input-group-btn").toggleClass("open");

        var $inputGroup = $link.closest(".input-group");
        var id = $link.data('id');
        var label = $link.html();
            
        $inputGroup.find('input[type="hidden"]').val(id);
        $inputGroup.find('input[type="text"]').val(label);
    });

};

function initConfirmDialogsEvents() {

    var okCallbackName = null;
    var okRedirectUri = null;
    var $confirmTrigger = null;

    $(document.body).on("click", ".confirm-trigger", function() {
        var $modal;
        okCallbackName = $(this).data("confirm-ok-callback");
        okRedirectUri = $(this).data("confirm-ok-redirect");
        $confirmTrigger = $(this);
        $.get("/commons/confirm", { confirmMessage: $(this).data("confirm-message") }, function (data) {
            $modal = $(data);
            $modal.modal();
        });
    })

    $(document.body).on("click", ".confirm-button-ok", function() {
        $(this).parents(".modal").modal('hide');
        if (okCallbackName) {
            var callback = window[okCallbackName];
            callback.call(this, $confirmTrigger);
        }
        if (okRedirectUri) {
            window.location.href = okRedirectUri;
        }
    });
}

function initDebugger() {
    $('.bb-messages-debugger-toggle').click(function() {
        var $butt = $(this).children('span');
        $('.bb-messages-debugger-content').toggle(0, function() {
            $(this).closest('.bb-messages-debugger').toggleClass("bb-messages-debugger-toggle-on");;
            $butt.toggleClass('glyphicon-chevron-right glyphicon-chevron-left');
        });
    });
}

function changeStateByAjax($trigger) {
    $.getJSON($trigger.data('change-state-url'), function () {
        window.location.reload();
    });
}
