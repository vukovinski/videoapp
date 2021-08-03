function wClass(element, className) {
  element.classList.add(className);
  return element;
}

function wAttribute(element, attributeName, attributeValue) {
  element.setAttribute(attributeName, attributeValue);
  return element;
}

function wChild(element, child) {
  element.appendChild(child);
  return element;
}

function makePlayer(trailer, w, h) {
  var player = wAttribute(wAttribute(wAttribute(wAttribute(wAttribute(wAttribute(
    document.createElement('iframe'),
    'width', `${w}`), 'height', `${h}`),
    'src', `https://www.youtube-nocookie.com/embed/${trailer.id.videoId}?controls=0&autoplay=1`),
    'title', 'Youtube video player'),
    'frameborder', '0'),
    'allow', 'accelerometer; autoplay; clipboard - write; encrypted - media; gyroscope; picture -in -picture; allowfullscreen');
  return player;
}

function withCarouselInner(carousel, trailers) {
  var inner_div = wClass(document.createElement('div'), 'carousel-inner');
  for (var i = 0; i < trailers.length; i++) {
    var item = wClass(document.createElement('div'), 'carousel-item');
    var item_img = wAttribute(wAttribute(wClass(wClass(document.createElement('img'), 'd-block'), 'w-100'), 'alt', `Trailer ${i + 1}`), 'mode', 'image');
    var item_caption_div = wClass(wClass(wClass(document.createElement('div'), 'carousel-caption'), 'd-none'), 'd-md-block');
    var item_caption_p = document.createElement('p'); item_caption_p.innerText = trailers[i].snippet.title;

    if (i === 0) {
      item = wClass(item, 'active');
    }

    const trailer = trailers[i];
    item_img.src = trailers[i].snippet.thumbnails.high.url;
    item_img.addEventListener('click', (e) => {
      var mode = e.target.getAttribute('mode');
      if (mode && mode === 'image') {
        var player = wAttribute(makePlayer(trailer, e.target.width, e.target.height), 'mode', 'video');
        e.target.parentNode.replaceChild(player, e.target);
      }
    });
    item = wChild(item, item_img);
    item_caption_div = wChild(item_caption_div, item_caption_p);
    item = wChild(item, item_caption_div);
    inner_div = wChild(inner_div, item);
  }
  return wChild(carousel, inner_div);
}

function withCarouselIndicators(carousel, trailers) {
  if (trailers && trailers.length > 1) {
    var indicators_div = wClass(document.createElement('div'), 'carousel-indicators');
    for (var i = 0; i < trailers.length; i++) {
      var indicator = wAttribute(wAttribute(wAttribute(wAttribute(
        document.createElement('button'),
        'type', 'button'),
        'data-bs-target', `#${carousel.id}`),
        'data-bs-slide-to', `${i}`),
        'aria-label', `Trailer ${i + 1}`);

      if (i === 0) {
        indicator = wClass(indicator, 'active');
      }

      indicator.addEventListener('click', (e) => {
        var index = e.target.getAttribute('data-bs-slide-to');
        bootstrap.Carousel.getOrCreateInstance(carousel).to(index);
      });
      indicators_div.appendChild(indicator);
    }
    return wChild(carousel, indicators_div);
  }
  return carousel;
}

function withCarouselControls(carousel, trailers) {
  if (trailers && trailers.length > 1) {
    var previous = wAttribute(wAttribute(wAttribute(wClass(
      document.createElement('button'),
      'carousel-control-prev'), 'type', 'button'),
      'data-bs-target', `#${carousel.id}`),
      'data-bs-slide', 'prev');
    var previous_icon = wAttribute(wClass(document.createElement('span'), 'carousel-control-prev-icon'), 'aria-hidden', 'true');
    var previous_span = wClass(document.createElement('span'), 'visually-hidden'); previous_span.innerText = 'Previous';
    previous = wChild(previous, previous_icon);
    previous = wChild(previous, previous_span);
    carousel.appendChild(previous);

    var next = wAttribute(wAttribute(wAttribute(wClass(
      document.createElement('button'),
      'carousel-control-next'), 'type', 'button'),
      'data-bs-target', `#${carousel.id}`),
      'data-bs-slide', 'next');
    var next_icon = wAttribute(wClass(document.createElement('span'), 'carousel-control-next-icon'), 'aria-hidden', 'true');
    var next_span = wClass(document.createElement('span'), 'visually-hidden'); next_span.innerText = 'Next';
    next = wChild(next, next_icon);
    next = wChild(next, next_span);
    carousel.appendChild(next);

    previous.addEventListener('click', (e) => {
      bootstrap.Carousel.getOrCreateInstance(carousel).prev();
    });

    next.addEventListener('click', (e) => {
      bootstrap.Carousel.getOrCreateInstance(carousel).next();
    });
  }
  return carousel;
}

function makeCarousel(movie, trailers) {
  var carousel_id = `carousel-${movie.imdbData.movie.title}`;
  var carousel = withCarouselControls(withCarouselIndicators(withCarouselInner(wAttribute(wClass(wClass(
    document.createElement('div'),
    'carousel'),
    'slide'),
    'data-bs-ride', 'carousel'), trailers), trailers), trailers);
  carousel.id = carousel_id;
  return carousel;
}

function showData(data) {
  var element = document.getElementsByClassName('results-view')[0];
  if (data && data.length > 0) {
    for (var i = 0; i < data.length; i++) {
      var movie = data[i];
      var movie_div = wClass(document.createElement('div'), 'movie');
      var movie_title = wClass(document.createElement('h3'), 'display-3'); movie_title.textContent = movie.imdbData.movie.title; movie_div.appendChild(movie_title);
      var movie_release = document.createElement('p'); movie_release.textContent = movie.imdbData.rating.year; movie_div.appendChild(movie_release);
      var movie_rating_imdb = document.createElement('p'); movie_rating_imdb.textContent = movie.imdbData.rating.imDb; movie_div.appendChild(movie_rating_imdb);
      var movie_description = document.createElement('p'); movie_description.textContent = movie.imdbData.title.plot; movie_div.appendChild(movie_description);
      var movie_carousel = makeCarousel(movie, movie.youtubeTrailersData.items); movie_div.appendChild(movie_carousel);

      element.appendChild(movie_div);
    }
  }
  else {
    element.innerHTML = '<p>Sorry, your search returned no results!</p>';
  }
}

var lastSearch = "";

async function search() {
  var searchString = document.getElementById('search').value;
  if (searchString && searchString !== "" && searchString !== lastSearch) {
    lastSearch = searchString;
    document.getElementById('spinner').classList.remove('visually-hidden');
    document.getElementsByClassName('results-view')[0].innerHTML = '';
    let response = await fetch(`/MovieTrailers?searchString=${searchString}`);
    if (response.ok) {
      showData(await response.json());
    }
    document.getElementById('spinner').classList.add('visually-hidden');
  }
}

function init() {
  var search_button = document.getElementById('search-button');
  search_button.addEventListener('click', async (e) => {
    await search();
  })
}

init();