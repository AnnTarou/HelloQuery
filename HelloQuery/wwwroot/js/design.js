// Lesson/Index.cshtml トグルバー開閉
$(document).ready(function () {
    $('#toggle-button').on('click', function () {
        $('#lesson-title').toggle();
    });
});
