// Class:   
//          DescriptorData
//          Descriptor queries 
// Constructor parameters:
//          db: The BrodySoft PhoneGap SQLlite plugin database reference or WebSQL database reference.

var DescriptorData = function (db) {


    this.linguaDB = db;

    this.configDB = new ConfigDB(this.linguaDB);
    this.phoneGapUtils = new PhoneGapUtils();

    //-------------------------------------------------------------------------------------------------------------

    // getDescriptor()
    //
    // Return the passed descriptor record
    this.getDescriptor = function (descriptor_id) {
        var deferred = new $.Deferred();
        var self = this;
        var i;
        

        this.linguaDB.transaction(function (tx) {

            tx.executeSql('SELECT * FROM descriptors WHERE id = ?', [descriptor_id], function (tx, res) {
                if (res.rows.length > 0) {

                    deferred.resolve({
                        "id": res.rows.item(i).id,
                        "code": res.rows.item(i).code,
                        "descriptor_name": res.rows.item(i).descriptor_name,
                        "other_flag": res.rows.item(i).other_flag,
                        "multi_select": res.rows.item(i).multi_select
                    });
                        
                   

                }
                else {
                    deferred.reject();
                }

                


            });


        }, function (e) {
            // Error in selecting version.
            self.phoneGapUtils.showAlert($.i18n('db-errdescget'));
            deferred.reject();
        });

        return deferred.promise();
    }

    //-------------------------------------------------------------------------------------------------------------
    
    // getDescriptorValues()
    //
    // Return the values of the passed descriptor
    this.getDescriptorValues = function (descriptor_id) {
        var deferred = new $.Deferred();
        var self = this;
        var i, retData;
        retData = [];

        this.linguaDB.transaction(function (tx) {

            tx.executeSql('SELECT * FROM descriptorvalues WHERE descriptor_id = ? and active = 1 order by sequence_no', [descriptor_id], function (tx, res) {
                if (res.rows.length > 0) {
                    
                    for (i = 0; i < res.rows.length; i++) {
                        retData.push({
                            "value_code": res.rows.item(i).value_code,
                            "value_name": res.rows.item(i).value_name
                        });
                    }

                }
               
                deferred.resolve(retData);
              
                
            });


        }, function (e) {
            // Error in selecting version.
            self.phoneGapUtils.showAlert($.i18n('db-errvalsget'));
            deferred.reject();
        });

        return deferred.promise();
    }


    //-------------------------------------------------------------------------------------------------------------

    // getDescriptorValueByCode()
    //
    // Get the descriptor value with the passed code
    this.getDescriptorValueByCode = function (value_code) {

        var deferred = new $.Deferred();
        var self = this;
        this.linguaDB.transaction(function (tx) {

            var sql = 'SELECT dv.id AS id, dv.descriptor_id AS descriptor_id, dv.value_code AS value_code, dv.value_name AS value_name, dv.sequence_no AS sequence_no, dv.active AS active, d.code AS code ' +
                    'FROM descriptorvalues AS dv, descriptors AS d '+
                    'WHERE dv.descriptor_id = d.id AND value_code = ?';

            tx.executeSql(sql, [value_code], function (tx, res) {
                if (res.rows.length === 0) {
                    self.phoneGapUtils.showAlert($.i18n('db-novalue'));
                    deferred.reject();
                }
                else {
                    deferred.resolve({
                        "id": res.rows.item(0).id,
                        "descriptor_id": res.rows.item(0).descriptor_id,
                        "value_code": res.rows.item(0).value_code,
                        "value_name": res.rows.item(0).value_name,
                        "sequence_no": res.rows.item(0).sequence_no,
                        "active": res.rows.item(0).active,
                        "code": res.rows.item(0).code
                    });
                }
            });


        }, function (e) {
            // Error in selecting version.
            self.phoneGapUtils.showAlert($.i18n('db-errvalget'));
            deferred.reject();
        });

        return deferred.promise();
    }
}