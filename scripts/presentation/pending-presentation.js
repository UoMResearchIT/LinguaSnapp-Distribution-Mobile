// Class:   
//          PendingPresentation
//          Presentation functionality for the 'show pending uploads' screen
// Constructor parameters:
//          db: The BrodySoft PhoneGap SQLlite plugin database reference or WebSQL database reference.
var PendingPresentation = function (db) {


    this.linguaDB = db;
    this.phoneGapUtils = new PhoneGapUtils();
    this.photoRecord = new PhotoRecord(this.linguaDB);

    // registerEvents
    // Register any events associated with the registration form
    this.registerEvents = function () {

        var self = this;

        $("#ulPending").on("tap", "li", function (e) {
            e.preventDefault();
           
            var photoRecordPres = new Photo1Presentation(self.linguaDB);
            $.mobile.loading('show', {
                text: $.i18n('lingua-loading'),
                textVisible: true

            });

            // If photo taken successfully redirect page to enter meta-data
            $(':mobile-pagecontainer').pagecontainer("change", "#photo1", { changeHash: true, unique_id: $(this).attr("id").substr(6) });
                
            // Initialise the form
            photoRecordPres.initialisePhotoForm();

            $.mobile.loading('hide');
            photoRecordPres.editRecordPage1($(this).attr("id").substr(6));

        });
        

    }

    //-------------------------------------------------------------------------------------------------------------
    // showAllRecords()
    //
    // Show all the photo record on the screen
    this.showAllRecords = function (data) {

        var self = this;
        
        $.each(data, function (index, photo) {
            
            var imageSize = Math.round(photo.image_data.length / 1024);
            
            var title = photo.title === null?"Untitled":photo.title;
           
            var created = new Date(photo.date_created);

            var commentsStr = "";
            if ((photo.latitude === 999 || photo.longitude === 999) && (photo.comments === null || photo.comments === "")) {
                commentsStr = "<br/><span style='color:Red; font-size:x-small'>" + $.i18n('pending-address') + "</span>";
            }

            if (photo.latitude !== 999 && photo.longitude !== 999) {
                commentsStr = ""; "<br/><span style='font-size:x-small'>GPS: " + photo.latitude.toFixed(8) + "N " + photo.longitude.toFixed(8) + "E" + "</span>";
            }
           
            $('#ulPending').append("<li   id='photo_"
                            + photo.unique_id
                            + "'  ><a  href='#' > <img style='width:60px; height:80px' src='data:image/jpeg;base64,"
                            + photo.image_data
                            +"'/> "
                            + title
                            + "<br/> <span style='font-size:small'>"
                            + created
                            + "<br/> <span style='font-size:small'>"
                            + $.i18n('pending-approxsize') + imageSize.toString() + "KiB"
                            + "</span>" + commentsStr + " </a></li>");

            

        });

        
        $('#ulPending').listview("refresh");
       
    }
}