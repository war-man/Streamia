class Searchia {
    constructor(typingDelay) {
        this.renderStyle();
        let searchia = document.getElementById('searchia');
        let searchiaDropdown = document.createElement('div');
        let searchiaInitialResult = document.createElement('div');

        searchiaDropdown.classList.add('searchia-dropdown', 'searchia-hidden');
        searchiaInitialResult.classList.add('searchia-result', 'searchia-hidden', 'searchia-init');
        searchiaInitialResult.innerHTML = 'Searching...';
        searchiaDropdown.appendChild(searchiaInitialResult);
        searchia.appendChild(searchiaDropdown);

        this.searchia = searchia;
        this.searchiaInitialResult = searchiaInitialResult;
        this.typingDelay = typingDelay || 500;
    }

    renderStyle() {
        document.body.innerHTML += `
            <style>
                #searchia {
                    position: relative;
                }

                #searchia .searchia-dropdown {
                    position: absolute;
                    z-index: 9999;
                    left: 0;
                    right: 0;
                    background-color: #FFF;
                    border: 1px solid #f1f1f1;
                    max-height: 250px;
                    overflow-y: auto;
                }

                #searchia .searchia-hidden {
                    display: none;
                }

                #searchia .searchia-result {
                    font-size: 16px;
                    padding: 10px 5px;
                    border-bottom: 1px solid #f1f1f1;
                    cursor: pointer;
                    transition: all .4s ease;
                }

                #searchia .searchia-result:last-child {
                    border-bottom: none;
                }

                #searchia .searchia-result:hover {
                    background-color: #f1f1f1;
                }
            </style>
        `;
    }

    toggleSearchDropdown(shouldShow) {
        if (shouldShow) {
            document.querySelector('.searchia-dropdown').classList.remove('searchia-hidden');
        } else {
            document.querySelector('.searchia-dropdown').classList.add('searchia-hidden');
        }
    }

    toggleSearchInitialResult(shouldShow) {
        if (shouldShow) {
            document.querySelector('.searchia-init').classList.remove('searchia-hidden');
        } else {
            document.querySelector('.searchia-init').classList.add('searchia-hidden');
        }
    }

    resetResults() {
        document.querySelector('.searchia-dropdown').querySelectorAll('.searchia-removable').forEach(removable => removable.remove());
        this.toggleSearchInitialResult(true);
    }

    setCallbackTimeout(callback) {
        this.timeout = setTimeout(callback, this.typingDelay);
    }

    clearCallbackTimeout() {
        this.timeout = null;
    }

    onTyping(callback) {
        let self = this;
        document.addEventListener('keyup', function (e) {
            if (e.target && e.target.parentElement.id == 'searchia') {
                self.resetResults();
                self.toggleSearchDropdown(true);
                if (!self.timeout) {
                    self.setCallbackTimeout(() => {
                        callback(e.target.value);
                        self.clearCallbackTimeout();
                    });
                }
            }
        });
    }

    onResult(id, text) {
        this.toggleSearchInitialResult(false);
        let searchResult = document.createElement('div');
        searchResult.classList.add('searchia-result', 'searchia-removable');
        searchResult.setAttribute('data-id', id);
        searchResult.innerHTML = text;
        document.querySelector('.searchia-dropdown').appendChild(searchResult);
    }

    onResultClick(callback) {
        let self = this;
        document.addEventListener('click', function (e) {
            if (e.target && e.target.classList.contains('searchia-removable')) {
                callback(e.target.getAttribute('data-id'));
                self.toggleSearchDropdown(false);
            }
        });
    }
}