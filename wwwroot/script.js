const cards = document.querySelectorAll(".card");
const letters = document.querySelectorAll(".letter");
const subtitle = document.querySelector(".subtitle");
const logo = document.querySelector(".logo");
const backgroundWatermark = document.querySelector(".background-watermark");
const ambientOne = document.querySelector(".ambient-1");
const ambientTwo = document.querySelector(".ambient-2");

if (window.gsap) {
  const tl = gsap.timeline({ defaults: { ease: "power3.out" } });

  tl.to(logo, {
    opacity: 1,
    y: 0,
    duration: 0.8
  })
    .to(letters, {
      opacity: 1,
      y: 0,
      scale: 1,
      duration: 0.8,
      stagger: 0.08
    }, "-=0.45")
    .to(subtitle, {
      opacity: 1,
      y: 0,
      duration: 0.65
    }, "-=0.3")
    .to(cards, {
      opacity: 1,
      y: 0,
      duration: 0.7,
      stagger: 0.12
    }, "-=0.2");

  gsap.to(logo, {
    y: -10,
    duration: 2.8,
    repeat: -1,
    yoyo: true,
    ease: "sine.inOut"
  });

  if (backgroundWatermark) {
    gsap.to(backgroundWatermark, {
      y: -14,
      scale: 1.03,
      rotation: 0.9,
      duration: 12,
      repeat: -1,
      yoyo: true,
      ease: "sine.inOut"
    });
  }

  gsap.to(ambientOne, {
    x: 50,
    y: 30,
    duration: 10,
    repeat: -1,
    yoyo: true,
    ease: "sine.inOut"
  });

  gsap.to(ambientTwo, {
    x: -40,
    y: -25,
    duration: 11,
    repeat: -1,
    yoyo: true,
    ease: "sine.inOut"
  });

  gsap.to(letters, {
    textShadow: "0 0 22px rgba(104, 220, 255, 0.38)",
    duration: 1.8,
    repeat: -1,
    yoyo: true,
    ease: "sine.inOut",
    stagger: {
      each: 0.06,
      from: "center"
    }
  });
}

cards.forEach((card) => {
  card.addEventListener("click", () => {
    const target = card.dataset.target;
    if (!target) return;

    card.style.transform = "translateY(-2px) scale(0.99)";
    setTimeout(() => {
      window.location.href = target;
    }, 160);
  });
});
