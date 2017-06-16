'use strict';

function PixellotWidgetAPI(config) { //jshint ignore: line
    var _self = this;
    var container = document.getElementById(config.container);
    var key = config.key;
    var origin = 'https://club.pixellot.tv'; // Dynamically change on request
    var isMobile = {
        Android: function () { return navigator.userAgent.match(/Android/i); },
        BlackBerry: function () { return navigator.userAgent.match(/BlackBerry/i); },
        iOS: function () { return navigator.userAgent.match(/iPhone|iPad|iPod/i); },
        Opera: function () { return navigator.userAgent.match(/Opera Mini/i); },
        Windows: function () { return navigator.userAgent.match(/IEMobile/i); },
        any: function () { return (isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Opera() || isMobile.Windows()); }
    };

    var mousedown = isMobile.any() ? "touchstart" : "mousedown";
    var mousemove = isMobile.any() ? "touchmove" : "mousemove";
    var mouseup = isMobile.any() ? "touchend" : "mouseup";

    var eventId;

    var widgetId = "pixellotWidget" + key;
    var playerId = "pixellotPlayer" + key;

    var widgetIframe;
    var playerWrapper;
    var playerBox;
    var playerIframe;

    var playerCursorStartPosition;
    var playerBoxStartPosition;

    function onPlayerMove(ev) {
        var playerBoxBoundingRectangle = playerBox.getBoundingClientRect();
        var clientX = ev.clientX || ev.touches[0].clientX;
        var clientY = ev.clientY || ev.touches[0].clientY;
        var playerShifts = [clientX - playerCursorStartPosition[0], clientY - playerCursorStartPosition[1]];
        var playerSize = [playerBoxBoundingRectangle.width, playerBoxBoundingRectangle.height];
        var windowSize = [window.innerWidth, window.innerHeight];
        var playerNewPosition = [playerBoxStartPosition[0] + playerShifts[0], playerBoxStartPosition[1] + playerShifts[1]];

        playerBox.style.left = Math.min(Math.max(0, playerNewPosition[0]), windowSize[0] - playerSize[0]) + "px";
        playerBox.style.top = Math.min(Math.max(0, playerNewPosition[1]), windowSize[1] - playerSize[1]) + "px";

        ev.preventDefault();
    }

    function onPlayerMoveEnd(ev) {
        playerCursorStartPosition = null;
        playerBoxStartPosition = null;
        playerIframe.style["pointer-events"] = "auto";

        document.removeEventListener(mousemove, onPlayerMove);
        document.removeEventListener(mouseup, onPlayerMoveEnd);

        ev.preventDefault();
    }

    /*** PLAYER MOVE ***/
    function onPlayerMoveStart(ev) {
        var playerBoxBoundingRectangle = playerBox.getBoundingClientRect();
        var clientX = ev.clientX || ev.touches[0].clientX;
        var clientY = ev.clientY || ev.touches[0].clientY;
        playerCursorStartPosition = [clientX, clientY];
        playerBoxStartPosition = [playerBoxBoundingRectangle.left, playerBoxBoundingRectangle.top];

        if (playerBox.style.margin === "auto") {
            playerBox.style.left = playerBox.offsetLeft + "px";
            playerBox.style.top = playerBox.offsetTop + "px";
            playerBox.style.position = "absolute";
            playerBox.style.margin = "";
        }

        playerIframe.style["pointer-events"] = "none";

        document.addEventListener(mousemove, onPlayerMove);
        document.addEventListener(mouseup, onPlayerMoveEnd);

        ev.preventDefault();
    }

    function onPlayerBoxMouseDown(ev) {
        var el = ev.target;
        var action = el.getAttribute("data-action");
        switch (action) {
            case "player-move":
                onPlayerMoveStart(ev);
                break;
            case "player-close":
                _self.closePlayer();
                break;
            case "player-detach-to-new-window":
                break;
            default:
                break;
        }
    }

    this.init = function () {
        _self.bind();
        _self.loadWidget();
    };

    this.bind = function () {
        window.addEventListener("message", function (ev) {
            var eventType = ev.data.type,
                eventDetails = ev.data.details,
                widgetKey = ev.data.widgetKey;

            switch (eventType) {
                case "openPlayer":
                    if (widgetKey !== key) {
                        return;
                    }

                    _self.openPlayer(eventDetails);
                    break;
                default:
                    break;
            }

        }, false);
    };

    this.loadWidget = function () {
        if (container) {
            widgetIframe = document.createElement('iframe');
            widgetIframe.id = widgetId;
            widgetIframe.style.width = "100%";
            widgetIframe.style.height = "250px";
            widgetIframe.style.border = "1px solid black";
            widgetIframe.style.overflow = "hidden";
            widgetIframe.style["-webkit-user-select"] = "none";
            widgetIframe.style["-moz-user-select"] = "none";
            widgetIframe.src = `${origin}/widget/${key}`;
            container.appendChild(widgetIframe);
        }
    };

    this.openPlayer = function (details) {
        var playerSrc = `${origin}/widget/${key}/player?eventId=${encodeURIComponent(details.eventId)}`;
        var playerHTML;

        if (container) {
            if (document.getElementById(playerId)) {
                if (eventId !== details.eventId) {
                    eventId = details.eventId;
                    playerIframe.src = playerSrc;
                }
            } else {
                eventId = details.eventId;
                playerHTML="Event Name: "
                playerHTML = '<div id="' + playerId + '_Wrapper" style="position: fixed; height: 100%; width: 100%; top: 0; left: 0; display: flex; z-index: 1000; pointer-events: none;">' +
                    '<div id="' + playerId + '_Box" style="position: relative; height: 450px; width: 730px; margin: auto; padding: 40px 10px 10px 10px; box-sizing: border-box; background-color: #1E1E1E; overflow: hidden; border-radius: 5px; pointer-events: auto;">' +
                    '<div data-action="player-move" style="position: absolute; height: 30px; top: 10px; left: 10px; right: 10px; cursor: move;"></div>' +
                    '<div data-action="player-close" style="position: absolute; height: 20px; width: 20px; top: 10px; right: 10px; cursor: pointer; background: url(\'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABEAAAATCAYAAAB2pebxAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA+lpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iIHhtbG5zOnhtcE1NPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvbW0vIiB4bWxuczpzdFJlZj0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL3NUeXBlL1Jlc291cmNlUmVmIyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgQ1M2IChNYWNpbnRvc2gpIiB4bXA6Q3JlYXRlRGF0ZT0iMjAxNi0wMi0wOVQxMDozNjo0MiswMjowMCIgeG1wOk1vZGlmeURhdGU9IjIwMTYtMDItMDlUMDk6MzQ6MzQrMDI6MDAiIHhtcDpNZXRhZGF0YURhdGU9IjIwMTYtMDItMDlUMDk6MzQ6MzQrMDI6MDAiIGRjOmZvcm1hdD0iaW1hZ2UvcG5nIiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOjdEMzI0RDU5QzczNDExRTU5RjRCQjEyQ0I5QUYyQjBGIiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOjdEMzI0RDVBQzczNDExRTU5RjRCQjEyQ0I5QUYyQjBGIj4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6N0QzMjRENTdDNzM0MTFFNTlGNEJCMTJDQjlBRjJCMEYiIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6N0QzMjRENThDNzM0MTFFNTlGNEJCMTJDQjlBRjJCMEYiLz4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz4kzH3ZAAAA1klEQVR42oxTARHDIBBjU4CESkBCJVRCJdRBJSBhEpiDzgF1UAmbAwY7dmPpB8hd7so35IH/v4QQVISJ1OqHPfKlZIywfqhkErmEf7gcR4q6UrCBYAKDIfJZ/E/fGk1QdHxFmY4lweOuILQ5PkF8K/dJ9/ZCRrzG0DIxoY4F9yhSBUsMvKRnJjo/LMJI+itpqNRod4jtmWeQk5je96hdxxOTU2WYySr0BO0RyYS1dnUk0ISJ6dygydyYZJzgG5roWqbKScfSxPWUUij9Z9KbE9pRPfsWYACdqOmJD7WvyQAAAABJRU5ErkJggg==\') center/65% no-repeat;"></div>' +
                    '<div data-action="player-detach-to-new-window" style="display: none; position: absolute; height: 20px; width: 20px; top: 5px; right: 30px; cursor: pointer; background: url(\'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAIAAAACACAYAAADDPmHLAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAIGNIUk0AAHolAACAgwAA+f8AAIDpAAB1MAAA6mAAADqYAAAXb5JfxUYAAAElSURBVHja7NtBDoAgDABBMP3/l+vZs02gMPMAI7ipF5iZORZb/gI3e2yBABAAAkAACAABIAAEgAAQAAJAAAgAASAABIAAEAACQAAIAAEgAASAABAAAkAACAAB0EGM/vfz5+XfME0ABIAAEAACQAAIAAEgAASAABAAAkAACAABIAAEgAAQAAIQAAJAAAgAASAABIAAEAAC4HhR8Iy/9/PTZzABEAACQAAIAAEgAASAABAAAkAACAABIABKxQFrcJ7ABEAACAABIAAEgAAQAAJAAAgAASAABIAA+NrhPMBsvofZef0mgF8AAkAACAABIAAEgAAQAAJAAAgAASAAjlRxHsD9fBMAASAABIAAEAACQAAIAAEgAASAABAAAmAvLwAAAP//AwDstAsEuWsjdgAAAABJRU5ErkJggg==\') center/75% no-repeat;"></div>' +
                    '<iframe id="' + playerId + '" src="' + playerSrc + '" style="height: 100%; width: 100%; border: none; -webkit-user-select: none; -moz-user-select: none;" allowfullscreen></iframe>' +
                    '</div>' +
                    '</div>';
                container.insertAdjacentHTML('beforeend', playerHTML);
                playerWrapper = document.getElementById(playerId + "_Wrapper");
                playerBox = document.getElementById(playerId + "_Box");
                playerIframe = document.getElementById(playerId);

                playerBox.addEventListener(mousedown, onPlayerBoxMouseDown);
            }
        }
    };

    this.closePlayer = function () {
        if (container && playerWrapper) {
            container.removeChild(playerWrapper);
            playerWrapper = null;
            playerIframe = null;
        }
    };
}