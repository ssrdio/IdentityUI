
class code2fa {
    constructor(AuthenticatorUri) {
        new QRCode(document.getElementById('qrCode'), {
            text: AuthenticatorUri,
            width: 150,
            height: 150
        });

        //Show/hide code
        $('.linkCode').on('click', function (event) {
            $('.code').addClass('showCode');
            $('.linkCode').addClass('hide');
        });

			$('#form2fa').find('input').val("");
    }
}



// input Formating

(function($, undefined) {
  'use strict';

  // When ready.
  $(function() {
      var $form = $('#form2fa');
    var $input = $form.find('input');

    $input.on('keyup', function(event) {
      // When user select text in the document, also abort.
      var selection = window.getSelection().toString();
      if (selection !== '') {
        return;
      }

      // When the arrow keys are pressed, abort.
      if ($.inArray(event.keyCode, [38, 40, 37, 39]) !== -1) {
        return;
      }

      var $this = $(this);
      var input = $this.val();
      input = input.replace(/[\W\s\._\-]+/g, '');

      var split = 3;
      var chunk = [];

      for (var i = 0, len = input.length; i < len; i += split) {
        // split = 4; //(i >= 4 && i <= 32) ? 4 : 8;
        chunk.push(input.substr(i, split));
      }

      $this.val(function() {
        return chunk.join('-').toUpperCase();
      });
    });

   
  });
})(jQuery);
