'use strict'; 


function execApiCall (url, method, data, callback) {
    console.log('calling ' + url);

    if (!callback) 
        callback = () => {};

    var options = {
        method: method,
        contentType: 'application/json',
        cache: false,
        success: function (result) {
            console.log(result);
            callback(result, null);
        },
        error: function (err) {
            console.log('ERROR:');
            console.log(err);
            callback(null, err);
        }
    };

    if (data) {
        options.dataType = 'json';
        options.data = JSON.stringify(data);
        //alert(options.data);
    }

    $.ajax(url, options);
}

function setFlag(flagName, value) {
    const js = {
        name: flagName, 
        value: value
    };

    execApiCall('http://localhost:8085/setFlag', 'POST', js, (output, error) => {
        if (error) {
            console.log(error); 
        }
        else {
            alert('sent');
        }
    });
}

$(document).ready(() => {
    $("#submitButton").click(() => {
        const text = $("#inputText").val(); 
        try {
            if (text && text.length) {
                const js = JSON.parse(text); 

                execApiCall('http://localhost:8085/sendData', 'POST', js, (output, error) => {
                    if (error) {
                        console.log(error); 
                        alert('an error has occurred'); 
                        alert(JSON.stringify(error));
                    }
                    else {
                        alert('sent');
                    }
                });
            }
        }
        catch(e) {
            console.error(e);
        }
    });

    $("#serverDown").change(function() {
        const checked = $('#serverDown').is(':checked');
        setFlag("SERVER_DOWN", checked);
    });

    $("#reconnect").change(function() {
        const checked = $('#reconnect').is(':checked');
        setFlag("RECONNECT", checked);
    });

    $("#authFailure").change(function() {
        const checked = $('#authFailure').is(':checked');
        setFlag("AUTH_FAILURE", checked);
    });
});