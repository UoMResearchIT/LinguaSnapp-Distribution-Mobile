// Class:   
//          configDB
//          Database layer and queries for the config table.
// Constructor parameters:
//          db: The BrodySoft PhoneGap SQLlite plugin database reference or WebSQL database reference.


var ConfigDB = function (db) {


    this.linguaDB = db;
    this.phoneGapUtils = new PhoneGapUtils();

    // This is the webservice location.
    this.serverLocation = "https://api.linguasnapp.com";


    // updateValue()
    // Params:
    //      key = config key name
    //      value = config key value 
    //
    // Updates the specified key in the config table with the specified value
    this.updateValue = function (key, value) {
        var deferred = new $.Deferred();
        var self = this;

        this.linguaDB.transaction(function (tx) {
            tx.executeSql('UPDATE config set value = ? WHERE key = ? ', [value, key]);
            tx.executeSql('INSERT INTO config (key, value) SELECT ?, ? WHERE NOT EXISTS(SELECT changes() AS change FROM config WHERE change <> 0);', [key, value]);
            deferred.resolve();
        }, function (e) {
            self.phoneGapUtils.showAlert("Error updating config table: key - " + key);
            deferred.reject();
        });

        return deferred.promise();
    }

    // getKey()
    // Params:
    //      key = config key name
    //
    // Return the value of the passed key
    this.getKey = function (key) {
        var deferred = new $.Deferred();
        var self = this;

        this.linguaDB.transaction(function (tx) {

            tx.executeSql('SELECT value FROM config WHERE key = ?', [key], function (tx, res) {
                if (res.rows.length === 0) {

                    deferred.reject();
                }
                else {
                    deferred.resolve({"value":res.rows.item(0).value});
                }
            });


        }, function (e) {
            // Error in getting key
           
            deferred.reject();
        });

        return deferred.promise();
    }


    // checkVersionCorrect()
    // Params:
    //      appVersion = version of the app source code (from populateDB.js)
    //
    // Check that the app version in the database matches that of the app source code, and if not reject the deferral
    this.checkVersionCorrect = function (appVersion) {
        var deferred = new $.Deferred();
        var self = this;
        
        this.linguaDB.transaction(function (tx) {
            
            tx.executeSql('SELECT value FROM config WHERE key = "version"', [], function (tx, res) {
                if (res.rows.length === 0 || res.rows.item(0).value !== appVersion) {
                  
                    deferred.reject();
                }
                else {
                    deferred.resolve();
                }
            });
            
            
        }, function (e) {
            // Error in selecting version.
            //self.phoneGapUtils.showAlert('Error checking version number');
            deferred.reject();
        });

        return deferred.promise();
    }


    // checkRegistered()
    //
    // Check that the app is registered, and if not reject the deferral
    this.checkRegistered = function () {
        var deferred = new $.Deferred();
        var self = this;

        this.linguaDB.transaction(function (tx) {

            tx.executeSql('SELECT value FROM config WHERE key = "registered"', [], function (tx, res) {
                if (res.rows.length === 0 || res.rows.item(0).value !== "yes") {
                    deferred.reject();
                }
                else {
                    deferred.resolve();
                }
            });

        }, function (e) {
            // Error in checking registration.
            self.phoneGapUtils.showAlert('Error checking registered');
            deferred.reject();
        });

        return deferred.promise();
    }

    // getUserID()
    //
    // Get the user ID from the config table
    this.getUserID = function () {
        var deferred = new $.Deferred();
        var self = this;
        var retData = [];

        this.linguaDB.transaction(function (tx) {

            tx.executeSql('SELECT value FROM config WHERE key = "userid"', [], function (tx, res) {
                if (res.rows.length === 0) {
                    
                    deferred.reject();
                }
                else {
                    $.extend(retData, {
                        "userid": res.rows.item(0).value
                    });

                    tx.executeSql('SELECT value FROM config WHERE key = "deviceid"', [], function (tx, res2) {
                        
                        if (res2.rows.length === 0){
                            deferred.reject();
                        }
                        else
                        {
                            $.extend(retData, {
                                "deviceid": res2.rows.item(0).value
                            });
                            deferred.resolve(retData);
                        }

                    });
                }

                    
            });
           

        }, function (e) {
            // Error in checking registration.
            self.phoneGapUtils.showAlert('Error checking for user ID');
            deferred.reject();
        });

        return deferred.promise();
    }
}