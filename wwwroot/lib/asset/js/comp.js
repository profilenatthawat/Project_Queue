var baseUrl = window.location.pathname;
var compName = baseUrl.substring(0, 7);
console.log(compName);
if (compName === "/repath") {
  $("#logo").attr("src", "/repatha/FrontEnd/dist/img/repatha.png");

  var color = "#9c0000";
  var rgbaCol =
    "rgba(" +
    parseInt(color.slice(-6, -4), 16) +
    "," +
    parseInt(color.slice(-4, -2), 16) +
    "," +
    parseInt(color.slice(-2), 16) +
    ",0.7)";
  $(".content-wrapper").css("background-color", rgbaCol);
  $(".col-12").css("background-color", "white");
  $(".form-group").css("background-color", "white");
  $(".pb-5, .py-5").css("background-color", "white");
  $(".border,.container").css("background-color", "white");
  $(
    "body > div.wrapper > div.content-wrapper > div > div.container-fluid.border.pt-2"
  ).css("background-color", "white");
  $(".text-danger").removeClass("background-color");
} else if (compName === "/amgevi") {
  console.log("defult color");
  $("#logo").attr("src", "/amgevita/FrontEnd/dist/img/amgevita.png");
} else {
  console.log("defult color");
  $("#logo").attr("src", "/amgevita/FrontEnd/dist/img/amgevita.png");
}
