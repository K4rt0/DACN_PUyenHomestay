$(function () {
  var r,
    t = $(".basic-ratings"),
    a = $(".custom-svg-ratings"),
    i = $(".multi-color-ratings"),
    n = $(".half-star-ratings"),
    e = $(".full-star-ratings"),
    s = $(".read-only-ratings"),
    l = $(".onset-event-ratings"),
    o = $(".onChange-event-ratings"),
    g = $(".methods-ratings"),
    c = $(".btn-initialize"),
    h = $(".btn-destroy"),
    f = $(".btn-get-rating"),
    w = $(".btn-set-rating");
  t && t.rateYo({ rating: 3.6, rtl: isRtl }),
    a &&
      a.rateYo({
        rating: 3.2,
        starSvg:
          "<svg xmlns='http://www.w3.org/2000/svg' width='24' height='24' viewBox='0 0 24 24'><path d='M12 6.76l1.379 4.246h4.465l-3.612 2.625 1.379 4.246-3.611-2.625-3.612 2.625 1.379-4.246-3.612-2.625h4.465l1.38-4.246zm0-6.472l-2.833 8.718h-9.167l7.416 5.389-2.833 8.718 7.417-5.388 7.416 5.388-2.833-8.718 7.417-5.389h-9.167l-2.833-8.718z'-></path>",
        rtl: isRtl,
      }),
    i && i.rateYo({ rtl: isRtl, multiColor: { startColor: "#fffca0", endColor: "#ffab00" } }),
    n && n.rateYo({ rtl: isRtl, rating: 2 }),
    e && e.rateYo({ rtl: isRtl, rating: 2 }),
    s && s.rateYo({ rating: 2, rtl: isRtl }),
    l &&
      l.rateYo({ rtl: isRtl }).on("rateyo.set", function (t, r) {
        alert("The rating is set to " + r.rating + "!");
      }),
    o &&
      o.rateYo({ rtl: isRtl }).on("rateyo.change", function (t, r) {
        r = r.rating;
        $(this).parent().find(".counter").text(r);
      }),
    g &&
      ((r = g.rateYo({ rtl: isRtl })),
      c &&
        c.on("click", function () {
          r.rateYo({ rtl: isRtl });
        }),
      h &&
        h.on("click", function () {
          r.hasClass("jq-ry-container") ? r.rateYo("destroy") : window.alert("Please Initialize Ratings First");
        }),
      f &&
        f.on("click", function () {
          var t;
          r.hasClass("jq-ry-container") ? ((t = r.rateYo("rating")), window.alert("Current Ratings are " + t)) : window.alert("Please Initialize Ratings First");
        }),
      w) &&
      w.on("click", function () {
        r.hasClass("jq-ry-container") ? r.rateYo("rating", 1) : window.alert("Please Initialize Ratings First");
      });
});
