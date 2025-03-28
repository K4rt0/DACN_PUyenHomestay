let t = document.querySelector(".credit-card-mask"),
  e = document.querySelector(".expiry-date-mask"),
  n = document.querySelector(".cvv-code-mask"),
  a = document.querySelector(".btn-reset"),
  o;
document.getElementById("addNewCCModal").addEventListener("show.bs.modal", function (e) {
  t &&
    (o = new Cleave(t, {
      creditCard: !0,
      onCreditCardTypeChanged: function (e) {
        document.querySelector(".card-type").innerHTML = "" != e && "unknown" != e ? '<img src="' + assetsPath + "img/icons/payments/" + e + '-cc.png" class="cc-icon-image" height="28"/>' : "";
      },
    }));
}),
  e && new Cleave(e, { date: !0, delimiter: "/", datePattern: ["m", "y"] }),
  n && new Cleave(n, { numeral: !0, numeralPositiveOnly: !0 }),
  FormValidation.formValidation(document.getElementById("addNewCCForm"), {
    fields: { modalAddCard: { validators: { notEmpty: { message: "Please enter your credit card number" } } } },
    plugins: { trigger: new FormValidation.plugins.Trigger(), bootstrap5: new FormValidation.plugins.Bootstrap5({ eleValidClass: "", rowSelector: ".col-12" }), submitButton: new FormValidation.plugins.SubmitButton(), autoFocus: new FormValidation.plugins.AutoFocus() },
    init: (e) => {
      e.on("plugins.message.placed", function (e) {
        e.element.parentElement.classList.contains("input-group") && e.element.parentElement.insertAdjacentElement("afterend", e.messageElement);
      });
    },
  }).on("plugins.message.displayed", function (e) {
    e.element.parentElement.classList.contains("input-group") && e.element.parentElement.insertAdjacentElement("afterend", e.messageElement.parentElement);
  }),
  a.addEventListener("click", function (e) {
    (document.querySelector(".card-type").innerHTML = ""), o.destroy();
  });
