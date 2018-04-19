// Class:   
//          PhotoRecordData
//          Save photo and record info
// Constructor parameters:
//          db: The BrodySoft PhoneGap SQLlite plugin database reference or WebSQL database reference.

var PhotoRecordData = function (db) {


    this.linguaDB = db;

    this.configDB = new ConfigDB(this.linguaDB);
    this.phoneGapUtils = new PhoneGapUtils();
    this.descriptorData = new DescriptorData(this.linguaDB);

    //-------------------------------------------------------------------------------------------------------------
    
    // saveNew()
    //
    // Save a new photo and return the ID of the photo record.
    this.saveNew = function (imageData, latitude, longitude) {

        var deferred = new $.Deferred();
        var self = this;

        this.linguaDB.transaction(function (tx) {

            var unique_id = self.phoneGapUtils.getGUID();

            var nowDate = new Date().toISOString();


            tx.executeSql('UPDATE photorecords set image_data = ?, latitude = ?, longitude = ?, date_created = ? WHERE unique_id = ? ', [imageData, latitude, longitude, nowDate, unique_id]);
            tx.executeSql('INSERT INTO photorecords  (unique_id, image_data, latitude, longitude, date_created) SELECT ?, ?, ?, ?, ? WHERE NOT EXISTS(SELECT changes() AS change FROM config WHERE change <> 0);', [unique_id, imageData, latitude, longitude, nowDate]);
            deferred.resolve({ "unique_id": unique_id });
        }, function (e) {
            self.phoneGapUtils.showAlert("Error inserting a new record into the photo table.");
            deferred.reject();
        });

        return deferred.promise();
    }

    //-------------------------------------------------------------------------------------------------------------
    
    // getRecord()
    //
    // Get the photo record with the passed ID
    this.getRecord = function (unique_id) {

        var deferred = new $.Deferred();
        var self = this;
        this.linguaDB.transaction(function (tx) {

            var retData;
            tx.executeSql('SELECT * FROM photorecords WHERE unique_id = ?', [unique_id], function (tx, res) {
                if (res.rows.length === 0) {
                    self.phoneGapUtils.showAlert('This photo no longer exists.');
                    deferred.reject();
                }
                else {
                    retData = {
                        "id": res.rows.item(0).id,
                        "unique_id": res.rows.item(0).unique_id,
                        "image_data": res.rows.item(0).image_data,
                        "latitude": res.rows.item(0).latitude,
                        "longitude": res.rows.item(0).longitude,
                        "date_created": res.rows.item(0).date_created,
                        "title": res.rows.item(0).title,
                        "comments": res.rows.item(0).notes
                    };

                    // get the descriptors too
                    var desc_select = 'SELECT pd.other_text AS other_text, pd.link_key AS link_key, dv.value_code AS value_code, dv.value_name AS value_name, d.code AS code ' +
                        'FROM photodescriptors AS pd, descriptors as d, descriptorvalues as dv ' +
                        'WHERE pd.descriptor_value_id = dv.id ' +
                        'AND dv.descriptor_id = d.id AND photo_record_id = ?';
                    tx.executeSql(desc_select, [res.rows.item(0).id], function (tx, res2) {
                        var index;
                        var descData = [];
                        for (index = 0; index < res2.rows.length; index++) {
                            descData.push({
                                "code": res2.rows.item(index).code,
                                "value_code": res2.rows.item(index).value_code,
                                "value_name": res2.rows.item(index).value_name,
                                "link_key": res2.rows.item(index).link_key,
                                "other_text": res2.rows.item(index).other_text
                            });
                        }

                        $.extend(retData, { "descriptors": descData });

                        // and the translations
                        var trans_select = 'SELECT translation, link_key  from phototranslations WHERE photo_record_id = ?';
                        tx.executeSql(trans_select, [res.rows.item(0).id], function (tx, res3) {
                            var index;
                            var transData = [];
                            for (index = 0; index < res3.rows.length; index++) {
                                transData.push({
                                    "translation": res3.rows.item(index).translation,
                                    "link_key": res3.rows.item(index).link_key
                                });
                            }

                            $.extend(retData, { "translations": transData });

                            deferred.resolve(retData);
                        });
                    });
                }
            });


        }, function (e) {
            // Error in selecting version.
            self.phoneGapUtils.showAlert('Error getting photo record.');
            deferred.reject();
        });
        
        return deferred.promise();
    }

    //-------------------------------------------------------------------------------------------------------------
    
    // savePhotoRecord()
    //
    // Save photo record page 1 data with the passed ID and data
    this.savePhotoRecord = function (unique_id, form_data) {
        var deferred = new $.Deferred();
        var self = this;

        var photo_promise = this.getRecord(unique_id); // Get the photo record for the ID
        photo_promise.done(function (photo_data) { // Got the photo record

            var def = [];
            self.linguaDB.transaction(function (tx) {


                // First update the title
                tx.executeSql('UPDATE photorecords set title = ? WHERE unique_id = ? ', [form_data.title, unique_id]);
            }, function (e) {
                self.phoneGapUtils.showAlert("Error updating photo record.");
                deferred.reject();
            });

           

            // Number of alphabets
            def.push(self.saveDescriptor(true, photo_data.id, "", form_data.numAlphabets, "NA001"));

            // Number of languages
            def.push(self.saveDescriptor(true, photo_data.id, "", form_data.numLanguages, "NL001"));
           
            $.each(form_data.languages, function (index, item) {
                // Update the language
                def.push(self.saveDescriptor(true, photo_data.id, item.link_key, item.other, item.language));
                
            });

            $.each(form_data.alphabets, function (index, item) {
                // Update the alphabet
                def.push(self.saveDescriptor(true, photo_data.id, item.link_key, item.other, item.alphabet ));
                
            });

            $.each(form_data.translations, function (index, item) {
                // Update the translation
                def.push(self.saveTranslation(photo_data.id, item.link_key, item.translation));
            });

            // Wait until they have finished then resolve
            $.when.apply($, def).done(function () {
                // Delete any language descriptors that need deleting
                var promise = self.deleteLanguageDescriptor(form_data.languages, photo_data.id);
                promise.done(function () {
                    deferred.resolve();
                });

                promise.fail(function () {
                    deferred.reject();
                });
                
            });
           

        });

        photo_promise.fail(function () { // Failed to get the photo record
            deferred.reject();
        });



        return deferred.promise();
    }

    //-------------------------------------------------------------------------------------------------------------

    // savePhotoRecord2()
    //
    // Save photo record page 2 data with the passed ID and data
    this.savePhotoRecord2 = function (unique_id, form_data) {
        var deferred = new $.Deferred();
        var self = this;

        var photo_promise = this.getRecord(unique_id); // Get the photo record for the ID
        photo_promise.done(function (photo_data) { // Got the photo record

            var def = [];
            self.linguaDB.transaction(function (tx) {


                // First update the comments
                tx.executeSql('UPDATE photorecords set notes = ? WHERE unique_id = ? ', [form_data.comments, unique_id]);
            }, function (e) {
                self.phoneGapUtils.showAlert("Error updating photo record.");
                deferred.reject();
            });



            // Position
            def.push(self.saveDescriptor(false, photo_data.id, "", "", form_data.position));

            // Sign type
            def.push(self.saveDescriptor(false, photo_data.id, "", "", form_data.signType));

            // Outlet
            def.push(self.saveDescriptor(false, photo_data.id, "", form_data.outletOther, form_data.outlet));

            // Contents
            def.push(self.saveMultiDescriptor(photo_data.id, "CO", form_data.contents));
            

            // Designs
            def.push(self.saveMultiDescriptor(photo_data.id, "DE", form_data.designs));

  
           

            // Wait until they have finished then resolve
            $.when.apply($, def).done(function () {
   
                deferred.resolve();
            });


        });

        photo_promise.fail(function () { // Failed to get the photo record
            deferred.reject();
        });



        return deferred.promise();
    }


    //-------------------------------------------------------------------------------------------------------------

    // savePhotoRecord3()
    //
    // Save photo record page 3 data with the passed ID and data
    this.savePhotoRecord3 = function (unique_id, form_data) {
        var deferred = new $.Deferred();
        var self = this;

        var photo_promise = this.getRecord(unique_id); // Get the photo record for the ID
        photo_promise.done(function (photo_data) { // Got the photo record

            var def = [];
           


            // Sector
            def.push(self.saveDescriptor(false, photo_data.id, "", "", form_data.sector));

            // Audience
            def.push(self.saveDescriptor(false, photo_data.id, "", "", form_data.audience));

            // One language dominant?
            def.push(self.saveDescriptor(false, photo_data.id, "", form_data.oneLang, "ON001"));

            // Purpose
            def.push(self.saveMultiDescriptor(photo_data.id, "PU", form_data.purposes));


            // Functions
            def.push(self.saveMultiDescriptor(photo_data.id, "FU", form_data.functions));

            // Arrangements
            def.push(self.saveMultiDescriptor(photo_data.id, "AR", form_data.arrangements));

            // Dominances
            def.push(self.saveMultiDescriptor(photo_data.id, "DO", form_data.dominances));


            // Wait until they have finished then resolve
            $.when.apply($, def).done(function () {

                deferred.resolve();
            });


        });

        photo_promise.fail(function () { // Failed to get the photo record
            deferred.reject();
        });



        return deferred.promise();
    }

    //-------------------------------------------------------------------------------------------------------------

    // deleteLanguageDescriptor()
    //
    // Delete any language/translation descriptors for the passed photo where the link_key is not in the passed list
    this.deleteLanguageDescriptor = function (languages, photo_id) {
        var deferred = new $.Deferred();
        var self = this;

        

        // create a list of all the link_keys on the current form.
        var link_keys = $.map(languages, function (n) {
            return "'" + n.link_key + "'";
        }).join(',');

        self.linguaDB.transaction(function (tx) {
           
            var sql, sql2;

            if (link_keys.length !== 0) {
                // delete everything not in the passed list
                sql = "DELETE FROM photodescriptors WHERE photo_record_id = " + photo_id + " AND link_key IS NOT NULL AND link_key != '' AND link_key NOT IN (" + link_keys + ")";
                sql2 = "DELETE FROM phototranslations WHERE photo_record_id = " + photo_id + " AND link_key IS NOT NULL AND link_key != '' AND link_key NOT IN (" + link_keys + ")";
            }
            else
            { // no link_keys at all so make sure all language/translations that are currently saved for this record are removed.
                sql = "DELETE FROM photodescriptors WHERE photo_record_id = " + photo_id + " AND link_key IS NOT NULL AND link_key != ''";
                sql2 = "DELETE FROM phototranslations WHERE photo_record_id = " + photo_id + " AND link_key IS NOT NULL AND link_key != ''";
            }

            tx.executeSql(sql);
            tx.executeSql(sql2);

            deferred.resolve();

        }, function (e) {
            self.phoneGapUtils.showAlert("Error deleting language/translation record.");
            deferred.reject();
        });

        return deferred.promise();
    }

    //-------------------------------------------------------------------------------------------------------------

    // saveDescriptor()
    //
    // Save descriptor data (not for multiselect dscriptors)
    this.saveDescriptor = function (lang_flag, photo_id, link_key, other_text, descriptor_code) {
        var ret = $.Deferred();
        var self = this;
        var alpha_promise = self.descriptorData.getDescriptorValueByCode(descriptor_code); // Get the descriptor record for the descriptor code

        alpha_promise.done(function (desc_data) {

            self.linguaDB.transaction(function (tx) {
                if (lang_flag === true) { // For saving a language/translation record
                    tx.executeSql('UPDATE photodescriptors set  other_text = ?, descriptor_code = ? WHERE photo_record_id = ? AND link_key = ? AND descriptor_value_id = ? ', [other_text, desc_data.code, photo_id, link_key, desc_data.id]);
                }
                else
                {
                    tx.executeSql('DELETE FROM photodescriptors  WHERE photo_record_id = ? AND descriptor_code = ? ', [photo_id, desc_data.code]);
                    tx.executeSql('UPDATE photodescriptors set  other_text = ?  WHERE descriptor_code = ? AND photo_record_id = ? AND descriptor_value_id = ? ', [other_text, desc_data.code, photo_id, desc_data.id]);

                }
                
                tx.executeSql('INSERT INTO photodescriptors  (photo_record_id, descriptor_value_id, descriptor_code, other_text, link_key) SELECT ?, ?, ?, ?, ? WHERE NOT EXISTS(SELECT changes() AS change FROM photodescriptors WHERE change <> 0);', [photo_id, desc_data.id, desc_data.code, other_text, link_key]);

                ret.resolve();

            }, function (e) {
                self.phoneGapUtils.showAlert("Error updating descriptor record.");
                ret.reject();
            });
        });

        alpha_promise.fail(function () {
            ret.reject();
        });

        return ret.promise();
    }

    //-------------------------------------------------------------------------------------------------------------

    // saveMultiDescriptor()
    //
    // Save descriptor data (for multiselect dscriptors)
    this.saveMultiDescriptor = function (photo_id, code, descriptor_code_arr) {
        var ret = $.Deferred();
        var self = this;
        self.linguaDB.transaction(function (tx) {
            tx.executeSql('DELETE FROM photodescriptors  WHERE photo_record_id = ? AND descriptor_code = ? ', [photo_id, code]);
        });

        if (descriptor_code_arr.length === 0) {
            return ret.resolve();
        }

        var count = 0;
       

        $.each(descriptor_code_arr, function (index, item) {
            var alpha_promise = self.descriptorData.getDescriptorValueByCode(item); // Get the descriptor record for the descriptor code

            alpha_promise.done(function (desc_data) {

                self.linguaDB.transaction(function (tx) {
                   

                    tx.executeSql('INSERT INTO photodescriptors  (photo_record_id, descriptor_value_id, descriptor_code, other_text, link_key) VALUES ( ?, ?, ?, ?, ?);', [photo_id, desc_data.id, desc_data.code, "", ""]);

                    count++;
                    if (count === descriptor_code_arr.length) {
                        ret.resolve();
                    }
                    

                }, function (e) {
                    self.phoneGapUtils.showAlert("Error updating descriptor record.");
                    ret.reject();
                });
            });

            alpha_promise.fail(function () {
                ret.reject();
            });
        });

        return ret.promise();
    }

    //-------------------------------------------------------------------------------------------------------------

    // saveTranslation()
    //
    // Save translation data
    this.saveTranslation = function (photo_id, link_key, translation) {
        var ret = $.Deferred();
        var self = this;

        self.linguaDB.transaction(function (tx) {
            // Update the translation
            tx.executeSql('UPDATE phototranslations set  translation = ? WHERE link_key = ? AND photo_record_id = ? ', [translation, link_key, photo_id]);
            tx.executeSql('INSERT INTO phototranslations  (photo_record_id, translation, link_key) SELECT ?, ?, ? WHERE NOT EXISTS(SELECT changes() AS change FROM phototranslations WHERE change <> 0);', [photo_id, translation, link_key]);

            ret.resolve();
        }, function (e) {
            self.phoneGapUtils.showAlert("Error updating translation record.");
            ret.reject();
        });

        return ret.promise();
    }

    //-------------------------------------------------------------------------------------------------------------

    // getAllRecords()
    //
    // Get all the photo records
    this.getAllRecords = function () {

        var deferred = new $.Deferred();
        var self = this;
        this.linguaDB.transaction(function (tx) {

            tx.executeSql('SELECT * FROM photorecords ORDER BY date_created', [], function (tx, res) {

                var retData = [];
                
                var index;
                for(index = 0; index< res.rows.length; index++) {
                   
                    retData.push({
                        "id": res.rows.item(index).id,
                        "unique_id": res.rows.item(index).unique_id,
                        "title": res.rows.item(index).title,
                        "image_data": res.rows.item(index).image_data,
                        "latitude": res.rows.item(index).latitude,
                        "longitude": res.rows.item(index).longitude,
                        "date_created": res.rows.item(index).date_created,
                        "comments": res.rows.item(index).notes
                    });

                    
                }

               
                deferred.resolve(retData);
            });
            


        }, function (error) {
            // Error in selecting version.
            self.phoneGapUtils.showAlert('Error getting photo records.' + error.message);
            deferred.reject();
        });

        return deferred.promise();
    }

    //-------------------------------------------------------------------------------------------------------------

    // deleteRecord()
    //
    // Delete the passed photo records
    this.deleteRecord = function (unique_id) {

        var deferred = new $.Deferred();
        var self = this;

        var promise = this.getRecord(unique_id); // Find the photo record

        promise.done(function (data) {
            // If the photo record was found then delete it.
            self.linguaDB.transaction(function (tx) {
                tx.executeSql('DELETE FROM photodescriptors  WHERE photo_record_id = ?', [data.id]); // delete the descriptors
                tx.executeSql('DELETE FROM phototranslations  WHERE photo_record_id = ?', [data.id]); // delete the translations
                tx.executeSql('DELETE FROM photorecords  WHERE id = ?', [data.id]); // delete the record itself
            });

            deferred.resolve();
        });

       
        promise.fail(function () {
            // Photo record was not found
            self.phoneGapUtils.showAlert('Error deleting photo record.');
            deferred.reject();   
        });

        return deferred.promise();
    }

}