// Lessons/Index.cshtml 問題が押されたときに表示される部分ビューを取得
// 同時にselected-lesson-idに選択されている問題のIDを格納
$('.lesson').on('click', function () {
    var lessonId = $(this).data('id');
    $.ajax({
        url: '/Lessons/SelectLesson',
        data: { id: lessonId },
        success: function (response) {
            $('#partialview-section').html(response);
        }
    });
});
