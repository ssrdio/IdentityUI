$(function() {
  $('.close-sidebar-btn').click(function() {
    var classToSwitch = $(this).attr('data-class');
    var containerElement = '.app-container';
    $(containerElement).toggleClass(classToSwitch);

    var closeBtn = $(this);

    if (closeBtn.hasClass('is-active')) {
      closeBtn.removeClass('is-active');
    } else {
      closeBtn.addClass('is-active');
    }
  });
});
