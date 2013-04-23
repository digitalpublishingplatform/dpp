var client;
window.console = window.console || { log: function () { } };

$(document).ready(function () {
    $('fieldset.formats input').live('change', function (e) {
        var checkBox = $(e.target),
            label = $('label[for=' + checkBox.attr('id') + ']');

        if (label) {
            if (checkBox.attr('checked')) {
                label.addClass('checked');
            } else {
                label.removeClass('checked');
            }
        }
    });

    $(".play-movie").live("click", function (e) {                                                                                                
        e.preventDefault();
        var $playerTrigger = $(this);
        var url = $(this).attr("href");
        $.get(url, function (result) {
            $("#video-container").html(result);
            setplayer();
            setCopyToClipboard();
            $("#video-container").dialog({
                height: "auto",
                width: "auto",
                modal: true,
                resizable: false,
                create: function (event, ui) {                    
                    var $mainDialog = $(event.target).closest(".ui-dialog");
                    $mainDialog.addClass("video-player-wrapper");                    
                    $mainDialog.find(".ui-dialog-title").html($mainDialog.find("h2.video-title").attr("title"));                    
                    var width = $("#video-container video").attr("width") - 20;
                    if (!width) {
                        width = $("#video-container object").attr("width") - 20;
                    }                                        
                    if (!width) {
                        width = $("#video-container .image-list-container").width() - 20;
                    }
                    $mainDialog.find(".ui-dialog-title").css("width", width);                    
                    $mainDialog.removeClass("ui-corner-all");                    
                    $(".video-player-wrapper .ui-dialog-titlebar").removeClass("ui-corner-all");                    
                    $(".video-player-wrapper .video-title").hide();
                },
                beforeClose: function (event, ui) {                    
                    $("#video-container").html("");
                    $("#video-container").dialog("destroy");
                }

            });
        });
        return false;
    });

    $(".digitalpublishingplatform .image-list li > a").live("click", function (event) {
        event.preventDefault();        
        $.get($(this).attr("href"), function (data) {
            if (data) {
                $("#video-container").dialog("close");
            }
        });
    });

    $(".digitalpublishingplatform .video-player-wrapper .left-button").live("click", function (event) {
        var container = $("#video-container .image-list-container");                
        container.stop(true, true).animate({ scrollLeft: "-=308" });
        event.preventDefault();
    });
    

    $(".digitalpublishingplatform .video-player-wrapper .right-button").live("click", function(event) {
        var container = $("#video-container .image-list-container");        
        container.stop(true, true).animate({ scrollLeft: "+=308" });
        event.preventDefault();
    });
    
    $(".digitalpublishingplatform .ui-widget-overlay").live("click", function () {
        $("#video-container").dialog("close");
        return false;
    });
    
    $(".digitalpublishingplatform .video-player-wrapper .image-list-container").live("mousewheel", function (event) {
        var delta = event.originalEvent.wheelDelta || -event.originalEvent.detail;
        if (delta > 0) {
            $(this).stop(true, true).animate({ scrollLeft: "+=308" });
        } else {
            $(this).stop(true, true).animate({ scrollLeft: "-=308" });
        }        
        event.preventDefault();
    });
    
    $(".digitalpublishingplatform .video-player-wrapper .scroll-button").live("mouseenter", function (e) {
        $(this).fadeTo("slow", 0.85);
    });
    
    $(".digitalpublishingplatform .video-player-wrapper .scroll-button").live("mouseleave", function (e) {
        $(this).fadeTo("slow", 0.5);
    });

    $(".digitalpublishingplatform .article-video-preview a.remove").live("click", function () {
        var $li = $(this).closest(".article-video-preview");
        var $hidden = $li.find("input");        
        $li.remove();
        window.renameAllInputs($(".digitalpublishingplatform .article-video-preview input"), "Videos.Ids[?]");
        return false;
    });
    
    $(".digitalpublishingplatform .article-image-preview a.remove").live("click", function () {
        var $li = $(this).closest(".article-image-preview");
        var $hidden = $li.find("input");      
        $li.remove();
        window.renameAllInputs($(".digitalpublishingplatform .article-image-preview input"), "ImageSet.Urls[?]");
        return false;
    });
    
    
    $(".digitalpublishingplatform fieldset.formats label.disabled").click(function (event) {
        event.preventDefault();
        return false;
    });

    $(document).bind("setTitle", function (e, data) {        
        var $title = $("#Title_Title");
        if ($title.length > 0 && $title.val() == "") {
            $title.val(data.title);
        }
    });

    $(".digitalpublishingplatform #add-image").live("click", function (e) {
                
        $(this).trigger("orchard-admin-pickimage-open", { img: null, uploadMediaPath: $(this).data("upload-media-folder"), callback: setImage });
        return false;
    });
    
    $(".digitalpublishingplatform #add-images").live("click", function (e) {

        $(this).trigger("orchard-admin-pickimage-open", { img: null, uploadMediaPath: $(this).data("upload-media-folder"), callback: setImages });
        return false;
    });

    if (jQuery().sortable) {
        $(".digitalpublishingplatform table#category tbody").sortable({
            cursor: "move",
            helper: function (e, ui) {
                ui.children().each(function () {
                    $(this).width($(this).width());
                });
                return ui;
            },
            stop: function (event, ui) {
                var values = $(".digitalpublishingplatform table#category tbody").sortable("serialize").replace(/\[\]/g, "");
                var i = 0;
                while (values.indexOf("?") > -1) {
                    values = values.replace("?", i);
                    i++;
                }
                updateCategoryOrder(parseQueryStringToArray(values));
                verifyCategoryOrder();
            }
        }).disableSelection();

        $(".digitalpublishingplatform #article-video-container ul").sortable({
            cursor: "move",
            stop: function (event, ui) {
                window.renameAllInputs($(".digitalpublishingplatform .article-video-preview input"), "Videos.Ids[?]");
            }
        });

        $(".digitalpublishingplatform #article-image-container ul").sortable({
            cursor: "move",
            stop: function (event, ui) {
                window.renameAllInputs($(".digitalpublishingplatform .article-image-preview input"), "ImageSet.Urls[?]");
            }
        });
    }
});

function setCopyToClipboard() {
    var $a = $(".digitalpublishingplatform #copy-container div.button-container > a");
    client = $.initcopytoclipboard($a,
        { moviePath: "/Modules/DigitalPublishingPlatform/content/ZeroClipboard.swf" },
        function () {
            $("#no-automatic-copy").show();
            $(".digitalpublishingplatform #copy-container div.button-container").hide();
            $(".digitalpublishingplatform #copy-container div.url-container").css("width", "99%");
            var $input = $(".digitalpublishingplatform #copy-container div.url-container > input");
            $input.focus();
            $input.select();
        },
        function () {
            var $textCopied = $(".digitalpublishingplatform .text-copied");
            $textCopied.stop(true, true).fadeIn("fast").fadeOut(3500);
        }
    );
    $a.live("click", function (e) {
        var $input = $(".digitalpublishingplatform #copy-container div.url-container > input");
        $input.focus();
        $input.select();
        client.copytoclipboard($input.val());
        return false;
    });

    $(".digitalpublishingplatform #copy-container div.url-container > input").bind("copy", function () {
        var $textCopied = $(".digitalpublishingplatform .text-copied");
        $textCopied.stop(true, true).fadeIn("fast").fadeOut(3500);
    });
}

function setImage(response) {
    var src = response.img.src;
    if (src.indexOf("http") < 0) {
        var origin = location.protocol + "//" + location.hostname;
        if (location.port) origin += ":" + location.port;
        src = origin + src;
    }
    $(".digitalpublishingplatform .image-container img").attr("src", src);
    $(".digitalpublishingplatform .image-container input[type=hidden]").val(src);
    $(".digitalpublishingplatform .image-container").show();
}

function setImages(response) {
    var src = response.img.src;
    if (src.indexOf("http") < 0) {
        var origin = location.protocol + "//" + location.hostname;
        if (location.port) origin += ":" + location.port;
        src = origin + src;
    }

    var urls = new Array();
    $(".digitalpublishingplatform #article-image-container input[type=hidden]").each(function() {
        urls.push($(this).val());
    });
    urls.push(src);
    addImages(urls);
}

function setplayer() {
        $('#video-player').mediaelementplayer({
            defaultVideoWidth: 600,
            defaultVideoHeight: 420,
            enablePluginDebug: false,
            autoplay: true,
            features: ['playpause', 'progress', 'current', 'duration', 'volume', 'fullscreen'],
            success: function (player, node) {
                if (!$.browser.msie) {                    
                    player.load();
                    player.play();
                }
            }
        });
}


function addImages(arrayOfValues) {
    $.ajax({
        type: "GET",
        url: "/Admin/DigitalPublishingPlatform/ImagePicker/ImageInfo",
        datatype: "json",
        traditional: true,
        data: $.param({ 'urls': arrayOfValues }, true),
        success: function (data) {
            $("#article-image-container ul").html(data);
        }
    });        
}

function updateCategoryOrder(arrayOfValues) {    
    $.ajax({
        type: "POST",
        url: location.href + "/UpdateCategoryOrder",
        datatype: "json",
        traditional: true,
        data: $.param({
            'ids': arrayOfValues,
            '__RequestVerificationToken': $('[name=__RequestVerificationToken]').val()
        }, true)
    });
}

function verifyCategoryOrder() {
    $(".digitalpublishingplatform table#category tr").find("img").removeClass("hide-move");
    $(".digitalpublishingplatform table#category tr:first-child").find(".move-up").addClass("hide-move");
    $(".digitalpublishingplatform table#category tr:last-child").find(".move-down").addClass("hide-move");
}

var parseQueryStringToArray = function (queryString) {
    var params = new Array();
    var queries, temp, i, l;

    // Split into key/value pairs
    queries = queryString.split("&");

    // Convert the array of strings into an object
    for (i = 0, l = queries.length; i < l; i++) {
        temp = queries[i].split('=');
        params[i] = temp[1];
    }

    return params;
};

