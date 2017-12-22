$(function () {
    $(window).scroll(function () {
        if ($(this).scrollTop() > 0) {
            $('#top').show();
        } else {
            $('#top').hide();
        }
    });
    $('#top').click(function () {
       // $(window).scrollTop(0);
        $('html,body').animate({ scrollTop: 0 }, 400);
    });
    $('#top').hover(
        function () { $(this).css('opacity', 1); },
        function () { $(this).css('opacity', 0.5); }
    );
});