// License
//
// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

function InputHover() {
$(document).ready(function(){
    $(".SearchTextBox").mouseover(function() { $(this).addClass('TextInputHover'); }).mouseout(function() { $(this).removeClass('TextInputHover'); }).focus( function() {	$(this).addClass('TextInputFocus'); }).blur( function() { $(this).removeClass('TextInputFocus') });
    $(".SearchDateBox").mouseover(function() { $(this).addClass('DateInputHover'); }).mouseout(function() { $(this).removeClass('DateInputHover'); }).focus( function() {	$(this).addClass('DateInputFocus'); }).blur( function() { $(this).removeClass('DateInputFocus') });
    $(".GridViewTextBox").mouseover(function() { $(this).addClass('TextInputhover'); }).mouseout(function() { $(this).removeClass('TextInputHover'); }).focus( function() {	$(this).addClass('TextInputFocus'); }).blur( function() { $(this).removeClass('TextInputFocus') });
});
}

function UserInformationLink_Hover(objectID) {
    $(document).ready(function() {
        $("#" + objectID).hover(
            function() { $(this).css("text-decoration", "underline"); }, 
            function() { $(this).css("text-decoration", "none"); 
        });
    });
}

/*
 * Fits a string to a pixel width and adds "..." to the end of the string to indicate the string has been truncated if it's too long.
 *
 * str    A string where html-entities are allowed but no tags.
 * width  The maximum allowed width in pixels
 * className  A CSS class name with the desired font-name and font-size. (optional)  
 *
 * From: http://stackoverflow.com/questions/282758/truncate-a-string-nicely-to-fit-within-a-given-pixel-width
 */

function fitStringToWidth(str,width,className) {

  // _escTag is a helper to escape 'less than' and 'greater than'
  function _escTag(s){ return s.replace("<","&lt;").replace(">","&gt;");}

  //Create a span element that will be used to get the width
  var span = document.createElement("span");
  //Allow a classname to be set to get the right font-size.
  if (className) span.className=className;
  span.style.display='inline';
  span.style.visibility = 'hidden';
  span.style.padding = '0px';
  document.body.appendChild(span);

  var result = _escTag(str); // default to the whole string
  span.innerHTML = result;
  // Check if the string will fit in the allowed width. NOTE: if the width
  // can't be determinated (offsetWidth==0) the whole string will be returned.
  if (span.offsetWidth > width) {
    var posStart = 0, posMid, posEnd = str.length, posLength;
    // Calculate (posEnd - posStart) integer division by 2 and
    // assign it to posLength. Repeat until posLength is zero.
    while (posLength = (posEnd - posStart) >> 1) {
      posMid = posStart + posLength;
      //Get the string from the begining up to posMid;
      span.innerHTML = _escTag(str.substring(0,posMid)) + '&hellip;';

      // Check if the current width is too wide (set new end)
      // or too narrow (set new start)
      if ( span.offsetWidth > width ) posEnd = posMid; else posStart=posMid;
    }

    result = '<abbr title="' +
      str.replace("\"","&quot;") + '">' +
      _escTag(str.substring(0,posStart)) +
      '&hellip;<\/abbr>';
  }
  document.body.removeChild(span);
  return result;
}

/*
 * CheckDateRange checks two dates and ensures that the first is less than the second.
 *
 * fromDate the date that should be less than the second date
 * toDate  the date that should be greater than the second date
 * textBoxId  the id of the textbox where the date value being checked is from
 * calendarExtenderId the calendar extender id where the date was selected from
 * message an error message that is displayed if the range is invalid
 *
 */

function CheckDateRange(fromDate, toDate, textBoxId, calendarExtenderId, message) {
    if(new Date(fromDate) > new Date(toDate)) {
        alert(message);
        document.getElementById(textBoxId).value='';
        $find(calendarExtenderId).set_selectedDate(null);
    }
}



