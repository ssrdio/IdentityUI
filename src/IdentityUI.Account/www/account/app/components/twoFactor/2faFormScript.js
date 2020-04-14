
class code2fa {
    constructor(AuthenticatorUri) {
        new QRCode(document.getElementById('qrCode'), {
            text: AuthenticatorUri,
            width: 150,
            height: 150
        });

        let showCodeText = 'Show Code';
        let hideCodeText = 'Hide Code';

        let showCode = false;

        let $code = $('.code');
        let $showCodeLink = $('.linkCode');

        //Show/hide code
        $('.linkCode').on('click', () => {
            if (showCode === true) {
                showCode = false;

                $code.hide();
                $showCodeLink.text(showCodeText);
            }
            else {
                showCode = true;

                $code.show();
                $showCodeLink.text(hideCodeText);
            }
        });

        const $form = $('#form2fa');

        var $input = $form.find('.verefication-code');

        $input.on('keyup', () => {
            // When user select text in the document, also abort.
            var selection = window.getSelection().toString();
            if (selection !== '') {
                return;
            }

            // When the arrow keys are pressed, abort.
            if ($.inArray(event.keyCode, [38, 40, 37, 39]) !== -1) {
                return;
            }

            var input = $input.val();
            input = input.replace(/[\W\s\._\-]+/g, '');

            var split = 3;
            var chunk = [];

            for (var i = 0, len = input.length; i < len; i += split) {
                // split = 4; //(i >= 4 && i <= 32) ? 4 : 8;
                chunk.push(input.substr(i, split));
            }

            $input.val(() => {
                return chunk.join('-').toUpperCase();
            });
        });
    }
}
