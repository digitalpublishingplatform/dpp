$(document).ready(function(){
    $("form").bind("videopicker-open", function (ev, data) {        
        data = data || {};
        // the popup will be doing full page reloads, so will not be able to retain
        // a pointer to the callback. We will generate a temporary callback
        // with a known/unique name and pass that in on the querystring so it
        // is remembers across reloads. Once executed, it calls the real callback
        // and removes itself.
        var callbackName = "_videopicker_" + new Date().getTime();
        data.callbackName = callbackName;
        $[callbackName] = function (returnData) {
            delete $[callbackName];
            data.callback(returnData);
        };
        $[callbackName].data = data;        
        var adminIndex = location.href.toLowerCase().indexOf("/digitalpublishingplatform/");
        if (adminIndex === -1) return;
        var url = location.href.substr(0, adminIndex)
            + "/DigitalPublishingPlatform/VideoPicker?"
            + "callback=" + callbackName            
            + "&" + (new Date() - 0);
        var w = window.open(url, "_blank", data.windowFeatures || "width=1000,height=700,status=no,toolbar=no,location=no,menubar=no,resizable=no");
    });

    $("#add-video").on("click", function () {        
        $("form").trigger("videopicker-open");
        return false;
    });

    $("button.close").click(function () {
        window.close();
    });

    $("#insert-video").on("click", function () {
        var selected = new Array();                
        $(".select-video:checked").each(function (index, checkbox) {            
            selected[index] = checkbox.value;
        });        
        window.opener.addVideo(selected);
        window.close();
    });
    window.addVideo = function (arrayOfValues) {        
        $.ajax({
            type: "GET",
            url: "/Admin/DigitalPublishingPlatform/VideoPicker/VideoInfo",
            datatype: "json",
            traditional: true,
            data: $.param({ 'ids': arrayOfValues }, true),
            success: function (data) {
                $("#article-video-container ul").append(data);
                window.renameAllInputs($(".digitalpublishingplatform .article-video-preview input"), "Videos.Ids[?]");
            }
        });        
        var newLi = $("<li></li>").html("");
        $("#article-video-container ul").append(newLi);
    };

    window.renameAllInputs = function($inputCollection, templateName) {
        $inputCollection.each(function(index, element) {
            element.name = templateName.replace("?", index);
        });
    };
});