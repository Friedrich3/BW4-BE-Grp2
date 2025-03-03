/*Click Immagine Dettaglio 
  Dev: WalterX95
*/

let mainProductImage = document.getElementById("mainProductImage");
let carouselImage = document.getElementsByClassName("carouselImage");

    for (let i = 0; i <= carouselImage.length; i++) {
        carouselImage[i].addEventListener("click", function () {
            mainProductImage.src = carouselImage[i].src;
        });
    }