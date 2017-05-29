

var ElementType =
{
    Empty: 0,
    Expression: 1,
    StartBlock: 2,
    EndBlock: 3,
    LogicOperator: 4
};

var startedBlocks = 0;
var gainedData = [];
var stacks = [];


function prepareStack(key) {

    var o = stacks[key];

    if (o == undefined)
        stacks[key] = [{ elementType: ElementType.Empty, value: '', userValue: ''}];
}

function addString(o, JeysonBox, TechPredicateBox, UserPredicateBox) {
    console.info("запустил addString");
    prepareStack(JeysonBox)

    var stack = stacks[JeysonBox];
    stack.push(o);

    var s = JSON.stringify(stack);

    JeysonBox = $('#' + JeysonBox);
    JeysonBox.val(s);

    var techBox = $('#' + TechPredicateBox);
    techBox.val(techBox.val() + o.value);

    var userBox = $('#' + UserPredicateBox);   
    userBox.val(userBox.val() + o.userValue);
    getUserPredicate(userBox);
} 

function addString2(o, TechPredicateBox, UserPredicateBox) {
    //console.info("запустил addString2");
    var techBox = $('#' + TechPredicateBox);
    techBox.val(techBox.val() + o.value);

    var userBox = $('#' + UserPredicateBox);
    userBox.val(userBox.val() + o.userValue);

    getUserPredicate(userBox);
}


function harvester(JeysonBox, TechPredicateBox, UserPredicateBox, ApplyButton, startBlockButton, endBlockButton, addExpressionButton, addAndOperatorButton, addOrOperatorButton) {
    //console.info("Запустил harvester");
    gainedData.push(
    {
        JeysonBox: JeysonBox,
        TechPredicateBox: TechPredicateBox,
        UserPredicateBox: UserPredicateBox,
        ApplyButton: ApplyButton,
        startBlockButton:  startBlockButton,
        endBlockButton: endBlockButton,
        addExpressionButton: addExpressionButton,
        addAndOperatorButton: addAndOperatorButton,
        addOrOperatorButton: addOrOperatorButton        
    });
}

 function PredicateReportViewExecutor() {
     //alert("запустил PredicateReportViewExecutor");
     for (var i = 0; i < gainedData.length; i++) {
         var element = gainedData[i];
         
         var stack = getStackFrom(element.JeysonBox);
         clearBoxes(element.TechPredicateBox, element.UserPredicateBox)

         for (var j = 0; j < stack.length; j++)
             addString2(stack[j], element.TechPredicateBox, element.UserPredicateBox)

         var elementType = stack[stack.length - 1].elementType;

         EnableButtons(element.ApplyButton, elementType, element.startBlockButton, element.endBlockButton, element.addExpressionButton, element.addAndOperatorButton, element.addOrOperatorButton);
     }
 }

 function PredicateDesignerExecutor() {
     console.info('Запустил DesignerExecutor');
    
     for (var i = 0; i < gainedData.length; i++) {

         var element = gainedData[i];

         var stack = getStackFrom(element.JeysonBox);

         clearBoxes(element.TechPredicateBox, element.UserPredicateBox);

         for (var j = 0; j < stack.length; j++)
             addString2(stack[j], element.TechPredicateBox, element.UserPredicateBox);

         var elementType = stack[stack.length - 1].elementType;

         EnableButtons(element.ApplyButton, elementType, element.startBlockButton, element.endBlockButton, element.addExpressionButton, element.addAndOperatorButton, element.addOrOperatorButton);
     }
 }



 function addExpression(JeysonBox, ApplyButton, startBlockButton, endBlockButton, addExpressionButton, addAndOperatorButton, addOrOperatorButton, TechPredicateBox, UserPredicateBox, OperatorList, ValueBox) {
     //alert("запустил addExpression");
     console.info('запустил addExpression');
     var lexem = $('#' + OperatorList).val();
     var value = $('#' + ValueBox).val();

     //var isChecked = $('#' + ValueBox).is(':checked');
     


     var result;
     var userResult;
     var column = '#a';
     var format;

     if (lexem.indexOf('NULL') > -1) {
         format = "{0} {1}";
         userResult = result = String.format(format, column, lexem);
     }
     else {
         if (value == '')
             return null;

         if (lexem.indexOf('like') > -1)
             value = String.format("%{0}%", value);
         
         format = "{0} {1} '{2}'";
         userResult = result = String.format(format, column, lexem, value);

         if (lexem.indexOf('<>') > -1 || lexem.indexOf('not like') > -1) {
             format = "({0} {1} '{2}')";
             result = String.format(format, column, lexem, value);
         }
     }
     var o = {
         elementType: ElementType.Expression,
         value: result,
         userValue: userResult
     };

     addString(o, JeysonBox, TechPredicateBox, UserPredicateBox);
    
     EnableButtons(ApplyButton, o.elementType, startBlockButton, endBlockButton, addExpressionButton, addAndOperatorButton, addOrOperatorButton);
 }
 
function startBlock(JeysonBox, TechPredicateBox, UserPredicateBox) {
    startedBlocks++;
    var o = {
        elementType: ElementType.StartBlock,
        value: '(',
        userValue: '('
    };

    addString(o, JeysonBox, TechPredicateBox, UserPredicateBox);  
}

function endBlock(JeysonBox, ApplyButton, startBlockButton, endBlockButton, addExpressionButton, addAndOperatorButton, addOrOperatorButton, TechPredicateBox, UserPredicateBox) {
    startedBlocks--;
    var o = {
        elementType: ElementType.EndBlock,
        value: ')',
        userValue: ')'
    };

    addString(o, JeysonBox, TechPredicateBox, UserPredicateBox);
   
    EnableButtons(ApplyButton, o.elementType, startBlockButton, endBlockButton, addExpressionButton, addAndOperatorButton, addOrOperatorButton); 
}

function addLogicOperator(JeysonBox, ApplyButton, startBlockButton, endBlockButton, addExpressionButton, addAndOperatorButton, addOrOperatorButton, TechPredicateBox, UserPredicateBox, logicOperator) {
    var o = {
        elementType: ElementType.LogicOperator,
        value: logicOperator,
        userValue: logicOperator
    }

    addString(o, JeysonBox, TechPredicateBox, UserPredicateBox);
    
    EnableButtons(ApplyButton, o.elementType, startBlockButton, endBlockButton, addExpressionButton, addAndOperatorButton, addOrOperatorButton);
}


function clearAllPredicate(JeysonBox, ApplyButton, startBlockButton, endBlockButton, addExpressionButton, addAndOperatorButton, addOrOperatorButton, TechPredicateBox, UserPredicateBox) {
   // alert("запустил clearAllPredicate");
    var techBox = $('#' + TechPredicateBox);
    techBox.val('');
    var userBox = $('#' + UserPredicateBox);
    userBox.val('');

    stacks[JeysonBox] = [{ elementType: ElementType.Empty, value: '', userValue: ''}];
    var stack = [{ elementType: ElementType.Empty, value: '', userValue: ''}];

    JeysonBox = $('#' + JeysonBox);
    JeysonBox.val(JSON.stringify(stack));

    var o = {
        elementType: ElementType.Empty,
        value: '',
        userValue: ''
    };

    EnableButtons(ApplyButton, o.elementType, startBlockButton, endBlockButton, addExpressionButton, addAndOperatorButton, addOrOperatorButton);

}

function clearPredicate(JeysonBox, ApplyButton, startBlockButton, endBlockButton, addExpressionButton, addAndOperatorButton, addOrOperatorButton, TechPredicateBox, UserPredicateBox) {

        
        var techBox = $('#' + TechPredicateBox);
        var userBox = $('#' + UserPredicateBox);
        var JeyBox = $('#' + JeysonBox);      

        prepareStack(JeysonBox)
        var stack = stacks[JeysonBox];

        var o = {
            elementType: stack[stack.length - 1].elementType,
            value: '',
            userValue: ''
        };

        if (stack.length > 1) {

            if (o.elementType == ElementType.StartBlock)
                startedBlocks--;

            else if (o.elementType == ElementType.EndBlock)
                startedBlocks++;

            stack.pop();
            o.elementType = stack[stack.length - 1].elementType;

            var newJsonString = JSON.stringify(stack);
            JeyBox.val(newJsonString);

            techBox.val('');
            userBox.val('');
            for (var i = 1; i < stack.length; i++) {
                var s = stack[i].value;
                var us = stack[i].userValue;
                techBox.val(techBox.val() + s);
                userBox.val(userBox.val() + us);
            }
            getUserPredicate(userBox);           
           
            var remaindElementType = stack[stack.length - 1].elementType;
           
            EnableButtons(ApplyButton, o.elementType, startBlockButton, endBlockButton, addExpressionButton, addAndOperatorButton, addOrOperatorButton);
    }

}


function EnableButtons(ApplyButton, elementType, startBlockButton, endBlockButton, addExpressionButton, addAndOperatorButton, addOrOperatorButton) {
    
    switch (elementType) {
        case ElementType.Expression:
            document.getElementById(addExpressionButton).disabled = true;
            document.getElementById(addAndOperatorButton).disabled = false;
            document.getElementById(addOrOperatorButton).disabled = false;
            document.getElementById(startBlockButton).disabled = true;
            document.getElementById(ApplyButton).disabled = false;
            if (startedBlocks > 0) {
                document.getElementById(endBlockButton).disabled = false;
                document.getElementById(ApplyButton).disabled = true;
            }
            break;

        case ElementType.EndBlock:
            document.getElementById(endBlockButton).disabled = startedBlocks == 0;
            document.getElementById(addExpressionButton).disabled = true;
            document.getElementById(addAndOperatorButton).disabled = false;
            document.getElementById(addOrOperatorButton).disabled = false;
            document.getElementById(startBlockButton).disabled = true;
            document.getElementById(ApplyButton).disabled = false;
            break;

        case ElementType.StartBlock:
            document.getElementById(addExpressionButton).disabled = false;
            document.getElementById(addAndOperatorButton).disabled = true;
            document.getElementById(addOrOperatorButton).disabled = true;
            document.getElementById(startBlockButton).disabled = false;
            document.getElementById(endBlockButton).disabled = true;
            document.getElementById(ApplyButton).disabled = true;
            break;

        case ElementType.LogicOperator:
            document.getElementById(addExpressionButton).disabled = false;
            document.getElementById(addAndOperatorButton).disabled = true;
            document.getElementById(addOrOperatorButton).disabled = true;
            document.getElementById(startBlockButton).disabled = false;
            document.getElementById(endBlockButton).disabled = true;
            document.getElementById(ApplyButton).disabled = true;
            break;

        case ElementType.Empty:
            document.getElementById(startBlockButton).disabled = false;
            document.getElementById(addExpressionButton).disabled = false;
            document.getElementById(addAndOperatorButton).disabled = true;
            document.getElementById(addOrOperatorButton).disabled = true;
            document.getElementById(endBlockButton).disabled = true;
            document.getElementById(ApplyButton).disabled = false;
            break;

        default:
            console.info('Нет обработки для элемента типа ' + elementType + '.');
    }
}

function getStackFrom(controlID) {

    var stack;
    var controlValue = $('#' + controlID).val();

    if (controlValue == '')
        stack = [{ elementType: ElementType.Empty, value: '', userValue: ''}];
    else stack = JSON.parse(controlValue);

    stacks[controlID] = stack;

    return stack;
}



function clearBoxes(TechPredicateBox, UserPredicateBox) {

    var techBox = $('#' + TechPredicateBox);
    techBox.val('');

    var userBox = $('#' + UserPredicateBox);
    userBox.val('');
}

String.format = function () {
    var theString = arguments[0];
    for (var i = 1; i < arguments.length; i++) {
        var regEx = new RegExp("\\{" + (i - 1) + "\\}", "gm");
        theString = theString.replace(regEx, arguments[i]);
    }
    return theString;
}

function getUserPredicate(userBox) {

    var userStr = $(userBox).val();
    userStr = userStr.replace(/like/gi, 'содержит');
    userStr = userStr.replace(/NULL/gi, 'пусто');
    userStr = userStr.replace(/IS/gi, '');
    userStr = userStr.replace(/NOT/gi, 'не');
    userStr = userStr.replace(/%/gi, '');
    userStr = userStr.replace(/AND/gi, 'И');
    userStr = userStr.replace(/OR/gi, 'ИЛИ');
    $(userBox).val(userStr);
}




