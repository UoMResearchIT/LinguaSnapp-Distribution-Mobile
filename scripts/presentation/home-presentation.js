// Class:   
//          HomePresentation
//          Presentation functionality for the main menu screen
// Constructor parameters:
//          db: The BrodySoft PhoneGap SQLlite plugin database reference or WebSQL database reference.
var HomePresentation = function (db) {


    this.linguaDB = db;
    this.phoneGapUtils = new PhoneGapUtils();

    // registerEvents
    // Register any events associated with the registration form
    this.registerEvents = function () {

        var self = this;

        // 'Take Photo' option on main screen chosen.
        $("#lnkTakePhoto").on("tap", function (e) {
            e.preventDefault();
           
            var photoRecord = new PhotoRecord(self.linguaDB);
            var photoRecordPres = new Photo1Presentation(self.linguaDB);
            $.mobile.loading('show', {
                text: "Saving photo.",
                textVisible: true

            });

            var promise = photoRecord.takePhoto();
            promise.done(function (data) {
                // If photo taken successfully redirect page to enter meta-data
                $(':mobile-pagecontainer').pagecontainer("change", "#photo1", { changeHash: true, unique_id: data.unique_id });
                $.mobile.loading('hide');
                

                // Initialise the form
                photoRecordPres.initialisePhotoForm();
                photoRecordPres.editRecordPage1(data.unique_id);
                
            });

            promise.fail(function () {
                $.mobile.loading('hide');
            });

        });

        // 'Show Pending' option on main screen chosen.
        $("#lnkShowPending").on("tap", function (e) {

            e.preventDefault();

            var photoRecord = new PhotoRecord(self.linguaDB);


            $.mobile.loading('show', {
                text: "Loading records.",
                textVisible: true

            });
            var promise = photoRecord.getAllRecords();
            promise.done(function (data) {
                // If photo taken successfully redirect page to enter meta-data
                $(':mobile-pagecontainer').pagecontainer("change", "#pendingUploads", { changeHash: true});
                $.mobile.loading('hide');
                var pending = new PendingPresentation(self.linguaDB);
                $("#ulPending").empty();
                pending.showAllRecords(data);
            });

        });

        // 'Upload Pending' option on main screen chosen.
        $("#lnkUploadPending").on("tap", function (e) {

            e.preventDefault();

            var upload = new Upload(self.linguaDB);


            $.mobile.loading('show', {
                text: "Uploading records.",
                textVisible: true

            });
            var promise = upload.uploadAllPhotos();
            promise.done(function (data) {
                
                $.mobile.loading('hide');
                
            });

        });

    }


    
}