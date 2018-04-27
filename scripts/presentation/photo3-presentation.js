// Class:   
//          Photo3Presentation
//          Presentation functionality for the third photo record edit screen
// Constructor parameters:
//          db: The BrodySoft PhoneGap SQLlite plugin database reference or WebSQL database reference.
var Photo3Presentation = function (db) {


    this.linguaDB = db;
    this.phoneGapUtils = new PhoneGapUtils();
    this.photoRecord = new PhotoRecord(this.linguaDB);
    this.descriptor = new Descriptor(this.linguaDB);
    this.upload = new Upload(this.linguaDB);

    // registerEvents
    // Register any events associated with the registration form
    this.registerEvents = function (e) {

        var self = this;

        // Save
        $("#lnkPhotoSave3").on("tap", function (e) {
            e.preventDefault();
            self.savePhoto("#pendingUploads")

        });

        // Go to first photo page
        $("#btn3Photo1").on("tap", function (e) {
            e.preventDefault();
            self.savePhoto("#photo1")

        });

        // Go to second photo page
        $("#btn3Photo2").on("tap", function (e) {
            e.preventDefault();
            self.savePhoto("#photo2")

        });

        // Flip switch changed
        $("#chkOneLang").on("change", function () {
            if ($(this).is(":checked")) {
                $("#divDominance").show();
            }
            else
            {
                $("#ddlDominance option:selected").removeAttr("selected");
                $("#ddlDominance").selectmenu("refresh");
                $("#divDominance").hide();
            }
        });

        // Delete photo 
        $("#lnkPhotoDelete3").on("tap", function (e) {
            e.preventDefault();
            self.deletePhoto();

        });

        // Show photo
        $("#lnkImagePage3").on("tap", function (e) {
            e.preventDefault();

            $(':mobile-pagecontainer').pagecontainer("change", "#showPhoto", { changeHash: true });

            var src = $("#imgPhoto3").prop("src");
            $('#imgBigImage').prop("src", src);
            $('#lnkBigImageBack').prop("href", "#photo3");
        });

        // Show about screen for page 3
        $("#btnAboutPage3").on("tap", function (e) {
            e.preventDefault();

            $(':mobile-pagecontainer').pagecontainer("change", "#aboutPage3", { changeHash: true });

          
        });

        // When clicking on the 'upload' button
        $("#btnUpload3").on("tap", function (e) {
            e.preventDefault();


            var isValid = $("#frmPhoto3").valid(); // Check the validity of the input

            if (isValid === true) {


                // Save the photo, passing all the currently selected data.
                var promise = self.photoRecord.savePhotoPage3();
                promise.done(function (data) {
                    var upload_promise = self.upload.uploadSinglePhoto($("#txtUniqueID3").text());

                    upload_promise.done(function () {
                        $(':mobile-pagecontainer').pagecontainer("change", "#main", { changeHash: false });
                    });

                });
            }
        });

        // When cancel is pressed just return to the uploads menu
        $("#lnkPhotoCancel3").on("tap", function (e) {
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

        $(document).on("pageinit", "#photo3", function () {
            $("#ddlPurpose-button").on("click", function () {
                setTimeout(self.changeIcon, 50);
            });


            $("#ddlFunction-button").on("click", function () {
                setTimeout(self.changeIcon, 50);
            });

            $("#ddlArrangement-button").on("click", function () {
                setTimeout(self.changeIcon, 50);
            });


            $("#ddlDominance-button").on("click", function () {
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
    // Save the third page of the photo record
    this.savePhoto = function (page_url) {
        var self = this;

        var isValid = $("#frmPhoto3").valid(); // Check the validity of the input

        if (isValid === true) {




            // Save the photo, passing all the currently selected data.
            var promise = self.photoRecord.savePhotoPage3();
            promise.done(function (data) {
                // If photo taken successfully redirect page to passed page
                $(':mobile-pagecontainer').pagecontainer("change", page_url, { changeHash: true, unique_id: data.unique_id });

                if (page_url === "#photo1") { // If page 1
                    var photo1Pres = new Photo1Presentation(self.linguaDB);

                    photo1Pres.initialisePhotoForm();
                    photo1Pres.editRecordPage1(data.unique_id);
                }

                if (page_url === "#photo2") { // If page 2
                    var photo2Pres = new Photo2Presentation(self.linguaDB);

                    photo2Pres.initialisePhotoForm();
                    photo2Pres.editRecordPage2(data.unique_id);
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
                var promise = self.photoRecord.deleteRecord($("#txtUniqueID3").text());
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
    // editRecordPage3
    //
    // Build up any screen elements and get any existing entered attributes for this record
    this.editRecordPage3 = function (unique_id) {
        var self = this;
        var promise = this.photoRecord.getRecord(unique_id);

        promise.done(function (data) {
            $('#btn3Photo3').addClass("ui-btn-active");
            $("#imgPhoto3").attr("src", "data:image/jpeg;base64," + data.image_data);

            $("#txtUniqueID3").text(unique_id);
            
            var oneLang = $.grep(data.descriptors, function (e) { return e.code === "ON"; });
            if (oneLang.length === 0 || oneLang[0].other_text === "no") {
                $("#chkOneLang").prop('checked', false);
                $("#chkOneLang").flipswitch().flipswitch('refresh');
                $("#ddlDominance option:selected").removeAttr("selected");
                $("#ddlDominance").selectmenu("refresh");
                $("#divDominance").hide();
            }
            else {
                $("#chkOneLang").prop('checked', true);
                $("#chkOneLang").flipswitch().flipswitch('refresh');
                $("#divDominance").show();
            }

            var sectorPromise = self.descriptor.buildDropDown(8); // Sector drop down
            sectorPromise.done(function (secHTML) {
                $("#ddlSector").append(secHTML);

                var sector = $.grep(data.descriptors, function (e) { return e.code === "SE"; });
                if (sector.length === 0) {
                    $("#ddlSector").val("SE!!");
                }
                else {
                    $("#ddlSector").val(sector[0].value_code);
                }

                $('#ddlSector').selectmenu('refresh');
            });

            var audPromise = self.descriptor.buildDropDown(9); // Audience drop down
            audPromise.done(function (audHTML) {
                $("#ddlAudience").append(audHTML);

                var audience = $.grep(data.descriptors, function (e) { return e.code === "AU"; });
                if (audience.length === 0) {
                    $("#ddlAudience").val("AU!!");
                }
                else {
                    $("#ddlAudience").val(audience[0].value_code);
                }

                $('#ddlAudience').selectmenu('refresh');
            });

            var purposePromise = self.descriptor.buildDropDown(10); // Purpose drop down
            purposePromise.done(function (purHTML) {
                $("#ddlPurpose").append(purHTML);
                var purposes = $.grep(data.descriptors, function (e) { return e.code === "PU"; });
                $.each(purposes, function (i, v) {

                    // mark options selected
                    $("#ddlPurpose option[value='" + v.value_code + "']").prop("selected", true);

                });
                $('#ddlPurpose').selectmenu('refresh');
            });

            var funPromise = self.descriptor.buildDropDown(11); // Function drop down
            funPromise.done(function (funHTML) {
                $("#ddlFunction").append(funHTML);

                var functions = $.grep(data.descriptors, function (e) { return e.code === "FU"; });
                $.each(functions, function (i, v) {

                    // mark options selected
                    $("#ddlFunction option[value='" + v.value_code + "']").prop("selected", true);

                });
                $('#ddlFunction').selectmenu('refresh');
            });

            var arrPromise = self.descriptor.buildDropDown(12); // Arrangement drop down
            arrPromise.done(function (arrHTML) {
                $("#ddlArrangement").append(arrHTML);

                var arrangements = $.grep(data.descriptors, function (e) { return e.code === "AR"; });
                $.each(arrangements, function (i, v) {

                    // mark options selected
                    $("#ddlArrangement option[value='" + v.value_code + "']").prop("selected", true);

                });

                $('#ddlArrangement').selectmenu('refresh');
            });


            var domPromise = self.descriptor.buildDropDown(14); // Dominance drop down
            domPromise.done(function (domHTML) {
                $("#ddlDominance").append(domHTML);

                var dominances = $.grep(data.descriptors, function (e) { return e.code === "DO"; });
                $.each(dominances, function (i, v) {

                    // mark options selected
                    $("#ddlDominance option[value='" + v.value_code + "']").prop("selected", true);

                });

                $('#ddlDominance').selectmenu('refresh');
            });

        });
    }

    //-------------------------------------------------------------------------------------------------------------

    // initialisePhotoForm()
    //
    // Initialise the photo form.
    this.initialisePhotoForm = function () {
        $("#ddlSector").empty();  // reset the sector dropdown

        $("#ddlAudience").empty();  // reset the audience drop down
        $("#ddlPurpose").empty();  // reset the purpose drop down

        $("#ddlFunction").empty();  // reset the function drop down

        $("#ddlArrangement").empty();  // reset the arrangement drop down
        $("#ddlDominance").empty();  // reset the dominance drop down
        $("#chkOneLang").attr('checked', false);
        $("#chkOneLang").flipswitch().flipswitch('refresh');
    }
}