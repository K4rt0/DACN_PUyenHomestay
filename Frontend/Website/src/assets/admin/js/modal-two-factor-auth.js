var n;
(n = document.querySelectorAll("#twoFactorAuthInputSms")) &&
  n.forEach(function (e) {
    new Cleave(e, { phone: !0, phoneRegionCode: "US" });
  });
