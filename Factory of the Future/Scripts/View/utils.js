function getHHMMSSFromSeconds(sec) {
    var utcString = (new Date(sec * 1000)).toUTCString();
    var match = utcString.match(/(\d+:\d\d:\d\d)/)[0];
    return match;
}