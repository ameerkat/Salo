var BotSelectorId = 0;
function addbot() {
    var s = $('<select name=\"BotSelector'+BotSelectorId+'\"/>');
    for (var val in window.BotJs) {
        $('<option />', { value: window.BotJs[val], text: window.BotJs[val] }).appendTo(s);
    }
    $('#bot-list').append("Player " + BotSelectorId + ":");
    s.appendTo("#bot-list");
    $('#bot-list').append("<br />");
    BotSelectorId++;
}