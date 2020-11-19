class DotLoader {
    constructor($loaderContainer, $dataContainer) {
        this.$dataContainer = $dataContainer;

        this.$loader = $loaderContainer.find('.dot-loader-holder');
    }

    show() {
        this.$dataContainer.hide();
        this.$loader.removeClass('dot-loader-hidden');
    }

    hide() {
        this.$dataContainer.show();
        this.$loader.addClass('dot-loader-hidden');
    }
}