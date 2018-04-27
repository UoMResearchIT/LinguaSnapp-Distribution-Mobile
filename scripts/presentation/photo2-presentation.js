// Class:   
//          Photo2Presentation
//          Presentation functionality for the second photo record edit screen
// Constructor parameters:
//          db: The BrodySoft PhoneGap SQLlite plugin database reference or WebSQL database reference.
var Photo2Presentation = function (db) {


    this.linguaDB = db;
    this.phoneGapUtils = new PhoneGapUtils();
    this.photoRecord = new PhotoRecord(this.linguaDB);
    this.descriptor = new Descriptor(this.linguaDB);
    this.upload = new Upload(this.linguaDB);

    // registerEvents
    // Register any events associated with the registration form
    this.registerEvents = function () {

        var self = this;

        // Save
        $("#lnkPhotoSave2").on("tap", function (e) {
            e.preventDefault();
            self.savePhoto("#pendingUploads")

        });

        // Go to first photo page
        $("#btn2Photo1").on("tap", function (e) {
            e.preventDefault();
            self.savePhoto("#photo1")

        });

        // Go to third photo page
        $("#btn2Photo3").on("tap", function (e) {
            e.preventDefault();
            self.savePhoto("#photo3")

        });

        // Delete photo 
        $("#lnkPhotoDelete2").on("tap", function (e) {
            e.preventDefault();
            self.deletePhoto();

        });

        // Show photo
        $("#lnkImagePage2").on("tap", function (e) {
            e.preventDefault();

            $(':mobile-pagecontainer').pagecontainer("change", "#showPhoto", { changeHash: true });

            var src = $("#imgPhoto2").prop("src");
            $('#imgBigImage').prop("src", src);
            $('#lnkBigImageBack').prop("href", "#photo2");
        });

        // Change outlet
        $("#ddlOutlet").on("change", function (e) {
            e.preventDefault();
            if ($('#ddlOutlet option:selected').val() === "OUOTHER") {
                $("#divOutletOther").show();
            }
            else {
                $("#txtOutletOther").val("");
                $("#divOutletOther").hide();
            }

        });

        // When clicking on the 'upload' button
        $("#btnUpload2").on("tap", function (e) {
            e.preventDefault();


            var isValid = $("#frmPhoto2").valid(); // Check the validity of the input

            if (isValid === true) {


                // Save the photo, passing all the currently selected data.
                var promise = self.photoRecord.savePhotoPage2();
                promise.done(function (data) {
                    var upload_promise = self.upload.uploadSinglePhoto($("#txtUniqueID2").text());

                    upload_promise.done(function () {
                        $(':mobile-pagecontainer').pagecontainer("change", "#main", { changeHash: false });
                    });

                });
            }
        });

        // When cancel is pressed just return to the uploads menu
        $("#lnkPhotoCancel2").on("tap", function (e) {
            e.preventDefault();
            $(':mobile-pagecontainer').pagecontainer("change", "#pendingUploads", { changeHash: false });
            var promise2 = self.photoRecord.getAllRecords();
            promise2.done(function (data) {
                $.mobile.loading('show', {
                    text: $.i18n('lingua-loading'),
                    textVisible: true

                });
                // If all records got successfully redirect page show the records

                $.mobile.loading('hide');
                var pending = new PendingPresentation(self.linguaDB);
                $("#ulPending").empty();
                pending.showAllRecords(data);
            });
        });

        $(document).on("pageinit", "#photo2", function () {
            $("#ddlContent-button").on("click", function () {
                setTimeout(self.changeIcon, 50);
            });


            $("#ddlDesign-button").on("click", function () {
                setTimeout(self.changeIcon, 50);
            });
        });


       
    }

    //-------------------------------------------------------------------------------------------------------------

    // changeIcon()
    //
    // Save the second page of the photo record
    this.changeIcon = function () {
       // alert("hello");
        $('.ui-popup .ui-header a, .ui-dialog .ui-header a').buttonMarkup({ icon: "check" });
        $('.ui-popup .ui-header a, .ui-dialog .ui-header a').text("Select");
        $('.ui-popup .ui-header a, .ui-dialog .ui-header a').removeClass("ui-btn-icon-notext").addClass("ui-btn-icon-left");
    }

    //-------------------------------------------------------------------------------------------------------------

    // savePhoto()
    //
    // Save the second page of the photo record
    this.savePhoto = function (page_url) {
        var self = this;

        var isValid = $("#frmPhoto2").valid(); // Check the validity of the input

        if (isValid === true) {

          


            // Save the photo, passing all the currently selected data.
            var promise = self.photoRecord.savePhotoPage2();
            promise.done(function (data) {
                // If photo taken successfully redirect page to passed page
                $(':mobile-pagecontainer').pagecontainer("change", page_url, { changeHash: true, unique_id: data.unique_id });

                if (page_url === "#photo1") { // If page 1
                    var photo1Pres = new Photo1Presentation(self.linguaDB);

                    photo1Pres.initialisePhotoForm();
                    photo1Pres.editRecordPage1(data.unique_id);
                }

                if (page_url === "#photo3") { // If page 3
                    var photo3Pres = new Photo3Presentation(self.linguaDB);

                    photo3Pres.initialisePhotoForm();
                    photo3Pres.editRecordPage3(data.unique_id);
                }

                if (page_url === "#pendingUploads") { // If pending uploads
                    var promise2 = self.photoRecord.getAllRecords();
                    promise2.done(function (data) {
                        $.mobile.loading('show', {
                            text: $.i18n('lingua-saving'),
                            textVisible: true

                        });
                        // If all records got successfully redirect page show the records
                       
                        $.mobile.loading('hide');
                        var pending = new PendingPresentation(self.linguaDB);
                        $("#ulPending").empty();
                        pending.showAllRecords(data);
                    });
                }

            });
        }
    }

    //-------------------------------------------------------------------------------------------------------------

    // deletePhoto()
    //
    // Delete the photo record
    this.deletePhoto = function (page_url) {
        var self = this;

        self.phoneGapUtils.showConfirm($.i18n('photo-deletesure'), function (buttonIndex) {


            if (buttonIndex === 1) { // Pressed OK


                // Delete the photo
                var promise = self.photoRecord.deleteRecord($("#txtUniqueID2").text());
                promise.done(function (data) {

                    $.mobile.loading('show', {
                        text: $.i18n('photo-deleting'),
                        textVisible: true

                    });
                    var promise2 = self.photoRecord.getAllRecords();
                    promise2.done(function (data) {
                        // If all records got successfully redirect page show the records
                        $(':mobile-pagecontainer').pagecontainer("change", "#pendingUploads", { changeHash: false });
                        $.mobile.loading('hide');
                        var pending = new PendingPresentation(self.linguaDB);
                        $("#ulPending").empty();
                        pending.showAllRecords(data);
                    });


                });
            }
        });
    }

    //-------------------------------------------------------------------------------------------------------------
    // editRecordPage2
    //
    // Build up any screen elements and get any existing entered attributes for this record
    this.editRecordPage2 = function (unique_id) {
        var self = this;
        var promise = this.photoRecord.getRecord(unique_id);

        promise.done(function (data) {
            $('#btn2Photo2').addClass("ui-btn-active");
            $("#imgPhoto2").attr("src", "data:image/jpeg;base64," + data.image_data);
           
            $("#txtUniqueID2").text(unique_id);
            $("#txtComments").val(data.comments);

           

            var posPromise = self.descriptor.buildDropDown(3); // Position drop down
            posPromise.done(function (posHTML) {
                $("#ddlPosition").append(posHTML);

                var position = $.grep(data.descriptors, function (e) { return e.code === "PO"; });
                if (position.length === 0) {
                    $("#ddlPosition").val("PO!!");
                }
                else {
                    $("#ddlPosition").val(position[0].value_code);
                }

                $('#ddlPosition').selectmenu('refresh');
            });

            var signPromise = self.descriptor.buildDropDown(4); // Sign type drop down
            signPromise.done(function (signHTML) {
                $("#ddlSignType").append(signHTML);

                var signType = $.grep(data.descriptors, function (e) { return e.code === "SI"; });
                if (signType.length === 0) {
                    $("#ddlSignType").val("SI!!");
                }
                else {
                    $("#ddlSignType").val(signType[0].value_code);
                }

                $('#ddlSignType').selectmenu('refresh');
            });

            var outPromise = self.descriptor.buildDropDown(5); // Outlet drop down
            outPromise.done(function (outHTML) {
                $("#ddlOutlet").append(outHTML);
                var outlet = $.grep(data.descriptors, function (e) { return e.code === "OU"; });
                if (outlet.length === 0) {
                    $("#ddlOutlet").val("OU!!");
                }
                else
                {
                    $("#ddlOutlet").val(outlet[0].value_code);

                    if (outlet[0].value_code === "OUOTHER") {
                        $("#divOutletOther").show();
                        $("#txtOutletOther").val(outlet[0].other_text);
                    }
                }
                $('#ddlOutlet').selectmenu('refresh');

                

            });

            var desPromise = self.descriptor.buildDropDown(6); // Design drop down
            desPromise.done(function (desHTML) {
                $("#ddlDesign").append(desHTML);

                var designs = $.grep(data.descriptors, function (e) { return e.code === "DE";});
                $.each(designs, function (i, v) {

                    // mark options selected
                    $("#ddlDesign option[value='" + v.value_code + "']").prop("selected", true);

                });
                $('#ddlDesign').selectmenu('refresh');
            });

            var conPromise = self.descriptor.buildDropDown(7); // Content drop down
            conPromise.done(function (conHTML) {
                $("#ddlContent").append(conHTML);

                var contents = $.grep(data.descriptors, function (e) { return e.code === "CO"; });
                $.each(contents, function (i, v) {

                    // mark options selected
                    $("#ddlContent option[value='" + v.value_code + "']").prop("selected", true);

                });

                $('#ddlContent').selectmenu('refresh');
                
            });

        });
    }

    //-------------------------------------------------------------------------------------------------------------

    // initialisePhotoForm()
    //
    // Initialise the photo form.
    this.initialisePhotoForm = function () {
        $("#ddlPosition").empty();  // reset the position dropdown
        
        $("#ddlSignType").empty();  // reset the sign type drop down
        $("#ddlOutlet").empty();  // reset the outlet drop down

        $("#ddlContent").empty();  // reset the content drop down

        $("#ddlDesign").empty();  // reset the design drop down

        $("#divOutletOther").hide();
        $("#txtOutletOther").val("");
        $("#txtComments").val("");
    }
}