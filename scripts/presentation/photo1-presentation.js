// Class:   
//          Photo1Presentation
//          Presentation functionality for the first photo record edit screen
// Constructor parameters:
//          db: The BrodySoft PhoneGap SQLlite plugin database reference or WebSQL database reference.
var Photo1Presentation = function (db) {


    this.linguaDB = db;
    this.phoneGapUtils = new PhoneGapUtils();
    this.photoRecord = new PhotoRecord(this.linguaDB);
    this.descriptor = new Descriptor(this.linguaDB);
    this.configDB = new ConfigDB(this.linguaDB);
    this.upload = new Upload(this.linguaDB);

    // registerEvents
    // Register any events associated with the registration form
    this.registerEvents = function () {

        var self = this;

        // Save
        $("#lnkPhotoSave1").on("tap", function (e) {
            e.preventDefault();
            self.savePhoto("#pendingUploads")

        });

        // Go to second photo page
        $("#btnPhoto2").on("tap", function (e) {
            e.preventDefault();
            self.savePhoto("#photo2")

        });

        // Go to third photo page
        $("#btnPhoto3").on("tap", function (e) {
            e.preventDefault();
            self.savePhoto("#photo3")

        });

        // When clicking on the 'add language' button
        $("#btnAddLang").on("tap", function () {
            self.addLanguageDiv();
        });

        // Delete photo 
        $("#lnkPhotoDelete1").on("tap", function (e) {
            e.preventDefault();
            self.deletePhoto();

        });

        // Show photo 
        $("#lnkImagePage1").on("tap", function (e) {
            e.preventDefault();
            
            $(':mobile-pagecontainer').pagecontainer("change", "#showPhoto", { changeHash: true});

            var src = $("#imgPhoto").prop("src");
            $('#imgBigImage').prop("src", src );
            $('#lnkBigImageBack').prop("href", "#photo1");
        });

        // Change language 
        $("#ddlLanguage").on("change", function (e) {
            e.preventDefault();
            if ($('#ddlLanguage option:selected').val() === "LAOTHER") {
                $("#divLanguageOther").show();
            }
            else {
                $("#txtLanguageOther").val("");
                $("#divLanguageOther").hide();
            }

        });

        // Change alphabet
        $("#ddlAlphabet").on("change", function (e) {
            e.preventDefault();
            if ($('#ddlAlphabet option:selected').val() === "ALOTHER") {
                $("#divAlphabetOther").show();
            }
            else {
                $("#txtAlphabetOther").val("");
                $("#divAlphabetOther").hide();
            }

        });

        // When clicking on the 'upload' button
        $("#btnUpload").on("tap", function (e) {
            e.preventDefault();
           

            var isValid = $("#frmPhoto1").valid(); // Check the validity of the input

            if (isValid === true) {


                // Save the photo, passing all the currently selected data.
                var promise = self.photoRecord.savePhotoPage1();
                promise.done(function (data) {
                    var upload_promise = self.upload.uploadSinglePhoto($("#txtUniqueID1").text());

                    upload_promise.done(function () {
                        $(':mobile-pagecontainer').pagecontainer("change", "#main", { changeHash: false });
                    });
                    
                });
            }
        });

        // When cancel is pressed just return to the uploads menu
        $("#lnkPhotoCancel1").on("tap", function (e) {
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

    }

    //-------------------------------------------------------------------------------------------------------------

    // savePhoto()
    //
    // Save the first page of the photo record
    this.savePhoto = function (page_url) {
        var self = this;

        var isValid = $("#frmPhoto1").valid(); // Check the validity of the input

        if (isValid === true) {

           
            // Save the photo, passing all the currently selected data.
            var promise = self.photoRecord.savePhotoPage1();
            promise.done(function (data) {
                // If all records got successfully redirect page show the records
                $(':mobile-pagecontainer').pagecontainer("change", page_url, { changeHash: true, unique_id: data.unique_id });

                if (page_url === "#photo2") { // If page 2
                    var photo2Pres = new Photo2Presentation(self.linguaDB);

                    photo2Pres.initialisePhotoForm();
                    photo2Pres.editRecordPage2(data.unique_id);
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

                self.configDB.updateValue("lastLang", $("#ddlLanguage").val());
                self.configDB.updateValue("lastAlpha", $("#ddlAlphabet").val());
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
                var promise = self.photoRecord.deleteRecord($("#txtUniqueID1").text());
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

    // addLanguageDiv()
    //
    // Add the values in the language box to the bottom of the screen
    this.addLanguageDiv = function () {
        var self = this;

        var lang_id = $("#ddlLanguage option:selected").val();
        var alpha_id = $("#ddlAlphabet option:selected").val();
        var translation = $("#txtTranslation").val();

        // In theory there should always be some selected options, but belt and braces...
        if (lang_id === "" && alpha_id === "" && translation === "") {
            this.phoneGapUtils.showAlert($.i18n('photo-enterlang'));
            
        }
        else {
            var link_key = this.phoneGapUtils.getGUID();
            $("#divLangNone").hide();
            this.appendLanguageDiv(link_key, lang_id, alpha_id, self.phoneGapUtils.htmlEscape($("#txtTranslation").val()), $("#ddlLanguage option:selected").text(), $("#ddlAlphabet option:selected").text(), self.phoneGapUtils.htmlEscape($("#txtLanguageOther").val()), self.phoneGapUtils.htmlEscape($("#txtAlphabetOther").val()));
           
        }



    }

    //-------------------------------------------------------------------------------------------------------------

    // appendLanguageDiv
    //
    // Build up any screen elements and get any existing entered attributes for this record
    this.appendLanguageDiv = function (link_key, lang_id, alpha_id, translation, lang_text, alpha_text, lang_other, alpha_other) {

        $("#divLangList").append("<div id='div_" + link_key + "'></div>");
        $("#div_" + link_key).append("<div class='vwLang' style='display:none' other_text='" + lang_other +"'>" + lang_id + "</div>");
        $("#div_" + link_key).append("<div class='vwAlpha' style='display:none'  other_text='" + alpha_other + "'>" + alpha_id + "</div>");
        $("#div_" + link_key).append("<div class='vwTrans' style='display:none'>" + translation + "</div>");

        var translation_txt = translation;
        if (translation === "") {
            translation_txt = "<em>" + $.i18n('photo-noneentered')+ "</em>";
        }

        $("#div_" + link_key).append("<div class='langListItem'>" +
                                        "<div style='width:90%; max-height: 100%; float:left; overflow: hidden'>" +
                                        "<strong>" + $.i18n('photo-language') + ": </strong>" + lang_text +
                                        "<br/>" +
                                        "<strong>" + $.i18n('photo-alphabet') + ": </strong>" + alpha_text +
                                        "<br/>" +
                                        "<strong>" + $.i18n('photo-translation') + ": </strong>" + translation_txt +
                                        "</div>" +
                                        "<div style='width:10%; float:right'>" +
                                        "<input type='button' data-role='button' data-icon='delete' data-iconpos='notext' onclick='$(\"#div_" + link_key + "\").remove()' id='btn_" + link_key + "'></input>" +
                                        "</div>");

        $("#btn_" + link_key).button().button('refresh');
    }

    //-------------------------------------------------------------------------------------------------------------

    // appendLanguageNone
    //
    // Show the fact that no languages/translations have been entered yet
    this.appendLanguageNone = function () {
        $("#divLangList").append("<div class='langListItem' id='divLangNone'>" +
                                       "<div style='width:100%; max-height: 100%; float:left; overflow: hidden'>" +
                                       $.i18n('photo-nolang') +
                                       "</div>" +
                                      
                                       "</div>");

       
    }

    //-------------------------------------------------------------------------------------------------------------

    // editRecordPage1
    //
    // Build up any screen elements and get any existing entered attributes for this record
    this.editRecordPage1 = function (unique_id) {
        var self = this;
        var promise = this.photoRecord.getRecord(unique_id);

        promise.done(function (data) {
            $('#btnPhoto1').addClass("ui-btn-active");
            $("#imgPhoto").attr("src", "data:image/jpeg;base64," + data.image_data);
            $("#txtTitle").val(data.title);
            $("#txtUniqueID1").text(unique_id);

            // Display the number of alphabets, if already saved.
            var result = $.grep(data.descriptors, function (e) { return e.code === "NA"; });
            if (result.length !== 0) {
                $("#numAlphabets").val(result[0].other_text);
            }

            // Display the number of languages, if already saved.
            var result2 = $.grep(data.descriptors, function (e) { return e.code === "NL"; });
            if (result2.length !== 0) {
                $("#numLanguages").val(result2[0].other_text);
            }

            var index;
            // Display any saved translations.
            var langs = $.grep(data.descriptors, function (e) { return e.code === "LA"; });
            if (langs.length !== 0) {
                for (index = 0; index < langs.length; index++) {
                    var alpha = $.grep(data.descriptors, function (e) { return e.code === "AL" && e.link_key === langs[index].link_key; });
                    var trans = $.grep(data.translations, function (e) { return e.link_key === langs[index].link_key; });

                    var ttrans = (trans.length === 0) ? "" : trans[0].translation;

                    self.appendLanguageDiv(langs[index].link_key, langs[index].value_code, alpha[0].value_code, ttrans, langs[index].value_name, alpha[0].value_name, langs[index].other_text, alpha[0].other_text);
                }
            }
            else
            {
                self.appendLanguageNone();
            }

            var langPromise = self.descriptor.buildDropDown(1); // Language drop down
            langPromise.done(function (langHTML) {
                $("#ddlLanguage").append(langHTML);

                var keyPromise = self.configDB.getKey("lastLang");
                keyPromise.done(function (valueData) {
                    $("#ddlLanguage").val(valueData.value);  // reset the language dropdown to the last found
                    $('#ddlLanguage').selectmenu('refresh');
                });
                
                keyPromise.fail(function () {
                    $("#ddlLanguage").val("LA001");  // reset the language dropdown to the default
                    $('#ddlLanguage').selectmenu('refresh');
                });
               
            });

            var alphaPromise = self.descriptor.buildDropDown(2); // Alphabet drop down
            alphaPromise.done(function (alphaHTML) {
                $("#ddlAlphabet").append(alphaHTML);
                var keyPromise = self.configDB.getKey("lastAlpha");
                keyPromise.done(function (valueData) {
                    $("#ddlAlphabet").val(valueData.value);  // reset the alphabet dropdown to the last found
                    $('#ddlAlphabet').selectmenu('refresh');
                });

                keyPromise.fail(function () {
                    $("#ddlAlphabet").val("AL001");  // reset the alphabet dropdown to the default
                    $('#ddlAlphabet').selectmenu('refresh');
                });
               
               
            });
        });
    }

    //-------------------------------------------------------------------------------------------------------------

    // initialisePhotoForm()
    //
    // Initialise the photo form.
    this.initialisePhotoForm = function () {
        $("#divLangList").empty(); // clear any existing translations
        //$("#txtTranslation").val(""); // clear the translation text
        $("#txtTitle").val(""); // clear the title text
        $("#ddlLanguage").empty();
        $("#ddlAlphabet").empty();
        //$("#divLanguageOther").hide();
        //$("#txtLanguageOther").val("");
        //$("#divAlphabetOther").hide();
        //$("#txtAlphabetOther").val("");
        $("#numAlphabets").val("0"); // clear the number of alphabets
        $("#numLanguages").val("0"); // clear the number of languages
    }
}