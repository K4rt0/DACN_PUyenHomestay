$(function () {
  var e = $(".select2");
  e.length &&
    e.each(function () {
      var e = $(this);
      e.wrap('<div class="position-relative"></div>').select2({ placeholder: "Select value", dropdownParent: e.parent() });
    });
});
var n, t;
(n = document.querySelector(".modal-edit-tax-id")), (t = document.querySelector(".phone-number-mask")), n && new Cleave(n, { prefix: "TIN", blocks: [3, 3, 3, 4], uppercase: !0 }), t && new Cleave(t, { phone: !0, phoneRegionCode: "US" });
