
function htmlmuban (mubanelem,) {


    var studenthtml = _.template($("#studenthtml").html());

    var studentdata = studenthtml({ studentnumber: data[0].StudentNumber, studentname: data[0].Name });

    $("#studenttable").append($(studentdata));

}


function formaDateUtc(dateUtc) {

    var date = new Date(parseInt(dateUtc));

    var year = date.getFullYear();

    var mouth = date.getMonth() + 1;

    var day = date.getDate();

    return year + "年" + mouth + "月" + day + "日";

}