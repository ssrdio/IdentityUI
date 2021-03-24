class DragAndDropComponent {
    constructor(addAvailableItem, addAssignedItem, availableItemsLabel, assignedItemsLabel) {
        this.statusAlert = new StatusAlertComponent('#status-alert');
        const $holder = $('#drag-and-drop-holder');
        this.loader = new DotLoader($('#drag-and-drop-loader'), $holder);

        this.addAvailableItem = addAvailableItem;
        this.addAssignedItem = addAssignedItem;

        new Sortable(assignedItems, {
            group: 'shared',
            animation: 150,
            forceFallback: true,
            ghostClass: 'sortable-ghost',
            filter: '.static',
            onAdd: (evt) => {
                var assginedItem = evt.item;
                var assginedItemId = $(assginedItem).data('id');
                this.addAssignedItem(assginedItemId);
            }
        });

        new Sortable(availableItems, {
            group: 'shared',
            animation: 150,
            ghostClass: 'sortable-ghost',
            forceFallback: true,
            filter: '.static',
            onAdd: (evt) => {
                var availableItem = evt.item;
                var availableItemId = $(availableItem).data('id');
                this.addAvailableItem(availableItemId);
            }
        });

        $("#available-items-label").text(availableItemsLabel);
        $("#assigned-items-label").text(assignedItemsLabel);
    }

    showErrors(errors) {
        if (errors[''] !== null && errors[''] !== undefined) {
            this.statusAlert.showErrors(errors['']);
        }
    }

    showError(error) {
        this.statusAlert.showError(error);
    }

    hideErrors() {
        this.statusAlert.hide();
    }

    hideLoader() {
        this.loader.hide();
    }

    initAvailableItems(data) {
        const availableItemsElement = document.getElementById('availableItems');
        availableItemsElement.innerHTML = '';

        data.forEach(item => {
            if (item.disabled) {
                availableItemsElement.insertAdjacentHTML('beforeend', this.getStaticDragItem(item));
            } else {
                availableItemsElement.insertAdjacentHTML('beforeend', this.getDragItem(item));
            }
        });
    }

    initAssignedItems(data) {
        const assignedItemsElement = document.getElementById('assignedItems');
        assignedItemsElement.innerHTML = '';

        data.forEach(item => {
            if (false) {
                assignedItemsElement.insertAdjacentHTML('beforeend', this.getStaticDragItem(item));
            } else {
                assignedItemsElement.insertAdjacentHTML('beforeend', this.getDragItem(item));
            }
        });
    }

    getDragItem(item) {
        return '<div class="drop-item" data-id=' + item.id + ' style="">' +
            '<div class="drop-item-container">' +
            '<p class="mb-0 d-inline-flex align-items-center">' +
            item.text +
            '</p>' +
            '<i class="fas fa-grip-vertical"></i>' +
            '</div>' +
            '</div>';
    }

    getStaticDragItem(item) {
        return '<div class="drop-item static" data-id=' + item.id + ' style="">' +
            '<div class="drop-item-container">' +
            '<p class="mb-0 d-inline-flex align-items-center">' +
            item.text +
            '</p>' +
            '</div>' +
            '</div>';
    }
}