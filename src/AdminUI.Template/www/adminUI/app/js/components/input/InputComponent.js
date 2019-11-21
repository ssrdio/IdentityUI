class InputComponent {

  constructor($parent, selector) {
    this.$el = $parent.find(selector);

    this.$input = this.$el.find('input');
    this.$error = this.$el.find('.error');
  }

  enable() {
    this.$input.prop('disabled', false);
  }

  disable() {
    this.$input.prop('disabled', true);
  }

  value(text) {
    if (text !== undefined) {
      this.$input.val(text);
      return text;
    }
    return this.$input.val();
  }

  show() {
    this.$el.show();
  }

  hide() {
    this.$el.hide();
  }

  clear() {
    this.value('');
  }

  showError(text) {
    this.$error.show();
    this.$error.text(text);
  }

  hideError() {
    this.$error.hide();
    this.$error.text('');
  }
}