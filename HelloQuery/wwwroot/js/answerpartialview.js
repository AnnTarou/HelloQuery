// 回答ページの部分ビュー

// HTMLのドキュメントが完全にリロードされたときに実行
$(function () {
    var id = '@Model';
    $.ajax({
        url: '/Lessons/GetAnswer',
        data: { id: id },
        success: function (response) {
            $('#partial-answerview-container').html(response);
        }
    });
});
