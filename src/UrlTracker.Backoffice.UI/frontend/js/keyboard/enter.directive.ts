export function ngEnterDirective(): angular.IDirective {
    return {
        link: (scope: angular.IScope, element: angular.IAugmentedJQuery, attributes: angular.IAttributes): void => {
            element.bind("keydown keypress", function (event) {
                if (event.which === 13) {
                    scope.$apply(function () {
                        scope.$eval(attributes.ngEnter)
                    });
                    event.preventDefault();
                }
            })
        }
    }
}