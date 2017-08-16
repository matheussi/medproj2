function mascara_DDDFONEFAX9(Campo, evt) {
    var tecla;

    if (evt.keyCode)
        tecla = evt.keyCode;
    else
        tecla = evt.which;

    if (tecla != 8 && tecla != 46) {
        var vr = new String(Campo.value);
        vr = vr.replace(".", "");
        vr = vr.replace(".", "");
        vr = vr.replace("/", "");
        vr = vr.replace("-", "");
        vr = vr.replace("(", "");
        vr = vr.replace(")", "");

        tam = vr.length + 1;
        if (tam > 2 && tam < 4)
            Campo.value = '(' + vr.substr(0, tam) + ')';

        if (tam > 6) {
            if (vr.substr(2, 1) == '9') {
                Campo.value = '(' + vr.substr(0, 2) + ')' + vr.substr(2, 5) + '-' + vr.substr(7, tam - 7);
            }
            else {
                if (tam == 11) {
                    if (evt.keyCode)
                        evt.returnValue = false;
                    else
                        evt.preventDefault();
                }

                Campo.value = '(' + vr.substr(0, 2) + ')' + vr.substr(2, 4) + '-' + vr.substr(6, tam - 6);
            }
        }
    }
}


function mascara_cel9(Campo, evt) {
    var tecla;

    if (evt.keyCode)
        tecla = evt.keyCode;
    else
        tecla = evt.which;

    var vr = new String(Campo.value);
    vr = vr.replace(".", "");
    vr = vr.replace(".", "");
    vr = vr.replace("/", "");
    vr = vr.replace("-", "");

    tam = vr.length + 1;

    if (tam > 4) {
        if (tam >= 10)
            Campo.value = vr.substr(0, 5) + '-' + vr.substr(5, tam);
        else {
            if (tam != 5)
                Campo.value = vr.substr(0, 4) + '-' + vr.substr(4, tam);
        }
    }
}

function mascara(campotexto) {
    try {
        var campo = document.getElementById(campotexto);
        var temp = campo.value;
        var re = new RegExp("\\D", "g");
        temp = temp.replace(re, "");
        var temp1 = temp.substr(0, temp.length - 2);
        var temp2 = temp.substr(temp.length - 2, 2);
        re = new RegExp("^0+");
        temp1 = temp1.replace(re, "");
        if (temp1 == "") { temp1 = "0"; }
        if (temp2.length == 0) { temp2 = "00"; }
        if (temp2.length == 1) { temp2 = "0" + temp2; }
        temp = temp1 + "," + temp2;
        campo.value = temp;
//        campo.focus();
    }
    catch (e) { alert(e); }
}

function filtro_SoNumeros(evt) {

    var tecla;

    if (evt.keyCode)
        tecla = evt.keyCode;
    else
        tecla = evt.which;

    if (tecla < 48 || tecla > 57) {
        if (evt.keyCode)
            evt.returnValue = false;
        else
            evt.preventDefault();
    }
}

function mascara_DATA(Campo, teclapres) {
    var tecla = teclapres.keyCode;

    var vr = new String(Campo.value);
    vr = vr.replace(".", "");
    vr = vr.replace("/", "");
    vr = vr.replace("-", "");

    tam = vr.length + 1;

    if (tecla != 9 && tecla != 8) {
        if (tam > 2 && tam < 4)
            Campo.value = vr.substr(0, 2) + '/' + vr.substr(3, tam);
        if (tam > 4 && tam < 11)
            Campo.value = vr.substr(0, 2) + '/' + vr.substr(2, 2) + '/' + vr.substr(5, tam - 4);
    }
}

function mascara_CNPJ(Campo, teclapres) {

    var tecla = teclapres.keyCode;

    var vr = new String(Campo.value);
    vr = vr.replace(".", "");
    vr = vr.replace(".", "");
    vr = vr.replace("/", "");
    vr = vr.replace("-", "");

    tam = vr.length + 1;

    if (tecla != 9 && tecla != 8) {
        if (tam > 2 && tam < 6)
            Campo.value = vr.substr(0, 2) + '.' + vr.substr(2, tam);
        if (tam >= 6 && tam < 9)
            Campo.value = vr.substr(0, 2) + '.' + vr.substr(2, 3) + '.' + vr.substr(5, tam - 5);
        if (tam >= 9 && tam < 13)
            Campo.value = vr.substr(0, 2) + '.' + vr.substr(2, 3) + '.' + vr.substr(5, 3) + '/' + vr.substr(8, tam - 8);
        if (tam >= 13 && tam < 15)
            Campo.value = vr.substr(0, 2) + '.' + vr.substr(2, 3) + '.' + vr.substr(5, 3) + '/' + vr.substr(8, 4) + '-' + vr.substr(12, tam - 12);
    }
}

function mascara_CEP(Campo, teclapres) {

    if (teclapres.keyCode == 8 || teclapres.keyCode == 46 || teclapres.keyCode == 37 || teclapres.keyCode == 39) {
        return;
    }

    var tecla = teclapres.keyCode;

    var vr = new String(Campo.value);
    vr = vr.replace(".", "");
    vr = vr.replace(".", "");
    vr = vr.replace("/", "");
    vr = vr.replace("-", "");

    tam = vr.length + 1;

    if (tam > 5)
        Campo.value = vr.substr(0, 5) + '-' + vr.substr(5, tam);
}

function mascara_CPF(Campo, teclapres) {
    var tecla = teclapres.keyCode;

    var vr = new String(Campo.value);
    vr = vr.replace(".", "");
    vr = vr.replace(".", "");
    vr = vr.replace("-", "");

    tam = vr.length + 1;

    if (tecla != 9 && tecla != 8) {
        if (tam > 3 && tam < 7)
            Campo.value = vr.substr(0, 3) + '.' + vr.substr(3, tam);
        if (tam >= 7 && tam < 10)
            Campo.value = vr.substr(0, 3) + '.' + vr.substr(3, 3) + '.' + vr.substr(6, tam - 6);
        if (tam >= 10 && tam < 12)
            Campo.value = vr.substr(0, 3) + '.' + vr.substr(3, 3) + '.' + vr.substr(6, 3) + '-' + vr.substr(9, tam - 9);
    }
}