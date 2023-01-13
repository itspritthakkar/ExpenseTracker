function getNotifications() {
    $.get(appUrl + "/api/Dashboard/Notifications", function (data, status) {
        if (status == "success") {
            if (data.data.length > 0) {
                var htmlString = "";
                data.data.forEach(function (ux) {
                    var date = new Date(ux.date);
                    htmlString += `<div class="dropdown-item">
                                <div class="media server-log">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-calendar"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"></rect><line x1="16" y1="2" x2="16" y2="6"></line><line x1="8" y1="2" x2="8" y2="6"></line><line x1="3" y1="10" x2="21" y2="10"></line></svg>
                                    <div class="media-body">
                                        <div class="data-info">
                                            <h6>Upcoming Expense</h6>
                                            <p>${ux.categoryName} - ${date.toLocaleDateString("en-IN")}</p>
                                        </div>
                                    </div>
                                </div>
                            </div>`
                });
                $(".notification-scroll").html(htmlString);
                $("#notification-status").toggleClass("d-none");
            }
        }
    });
}

function imagePreviewer() {
    //First upload
    var iconInput = document.getElementById("Icon");
    if(iconName == ''){iconName = null}
    if(iconName == null){
        var firstUpload = new FileUploadWithPreview('myFirstImage');
    }
    else{
        var firstUpload = new FileUploadWithPreview('myFirstImage', {
            presetFiles: [iconName]
        });
    }

    window.addEventListener('fileUploadWithPreview:imagesAdded', function (e) {
        var upIconName = e.detail.cachedFileArray[0].name;
        if(upIconName != iconName){
            iconInput.value = upIconName;
        }
    })
    document.querySelector(".custom-file-container__image-clear").addEventListener("click", function (e) {
        iconInput.value = "";
    });
}

function serverValidation(xhr, status, error) {
    var err = xhr.responseJSON;
    Object.keys(err.errors).forEach(key => {
        $("span[data-attribute="+key+"]").text(err.errors[key][0]);
    });
}

$(document).ready(function () {
    $(".sidebarCollapse").each(function () {
        $(this).on("click", function (sidebar) {
            sidebar.preventDefault();
            $(".main-container").toggleClass("sidebar-closed");
            $(".header").toggleClass('expand-header');
            $(".main-container").toggleClass("sbar-open");
            $('html,body').toggleClass('sidebar-noneoverflow');
            $(".header-container").toggleClass("sidebar-closed");
        });
    });

    getNotifications();
});