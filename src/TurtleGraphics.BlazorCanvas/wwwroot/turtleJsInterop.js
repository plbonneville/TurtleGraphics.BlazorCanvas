// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

export let canvasContainer = null;
export let canvas = null;
export let instance = null;

export function step(timeStamp) {
    window.requestAnimationFrame(step);
    instance.invokeMethodAsync('Step', timeStamp);
}

export function onResize() {
    if (!canvas || !canvasContainer)
        return;

    //canvas.width = window.innerWidth;
    //canvas.height = window.innerHeight;
    //console.log("window.innerWidth: " + window.innerWidth + " canvasContainer.clientWidth: " + canvasContainer.clientWidth);

    console.log("window.innerWidth" + window.innerWidth);
    console.log("canvasContainer.offsetWidth" + canvasContainer.offsetWidth);
    console.log("canvasContainer.clientWidth" + canvasContainer.clientWidth);
    console.log("canvasContainer.scrollWidth" + canvasContainer.scrollWidth);

    instance.invokeMethodAsync('OnResize', canvasContainer.clientWidth, canvas.height);
}

export function init(dotNetObject) {
    canvasContainer = document.getElementById('canvasContainer');
    let canvases = canvasContainer.getElementsByTagName('canvas') || [];

    instance = dotNetObject;
    canvas = canvases.length ? canvases[0] : null;
    window.requestAnimationFrame(step);

    onResize();
    window.addEventListener("resize", onResize);
}