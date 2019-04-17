$(function() {
    // run this on documentload:
    var canvas = $("#canvas");
    
    //hooks
    canvas.mousemove(function(e) { handleMouseMove(e); });
    canvas.mousedown(function(e) { handleMouseDown(e); });
    canvas.mouseup(function(e) { handleMouseUp(e); });
    
    var ctx = canvas[0].getContext("2d");
    var resizedWidth = 0.0;

    var img = new Image();
    img.onload = function() {
        // hook this onload
        canvas[0].width = img.width;
        resizedWidth = img.width / canvas[0].offsetWidth;
        canvas[0].height = img.height;
        ctx.drawImage(img, 0, 0);
    };
    img.src = imgSrc;

    var x1 = 0;
    var y1 = 0;
    var x2 = 0;
    var y2 = 0;
    var mousePressed = false;

    function handleMouseMove(e) {
        var mouseLoc = mouseOnImageLoc(e);

        // move rectangle second corner, if mouse is pressed.
        if (mousePressed) {
            x2 = mouseLoc[0];
            y2 = mouseLoc[1];
        }

        // clear the canvas by drawing image
        ctx.drawImage(img, 0, 0);

        // draw lines crossing in mouseposition
        ctx.beginPath();
        ctx.strokeStyle = "#FFFF00";
        ctx.lineWidth = 2;
        ctx.moveTo(mouseLoc[0], 0);
        ctx.lineTo(mouseLoc[0], canvas[0].height);
        ctx.moveTo(0, mouseLoc[1]);
        ctx.lineTo(canvas[0].width, mouseLoc[1]);
        ctx.stroke();
        ctx.closePath();

        // draw rectangle of current box
        ctx.beginPath();
        ctx.strokeStyle = "#FF0000";
        ctx.lineWidth = 5;
        ctx.rect(x1, y1, x2 - x1, y2 - y1);
        ctx.stroke();
        ctx.closePath();
    }

    function handleMouseDown(e) {
        var mouseLoc = mouseOnImageLoc(e);
        x1 = x2 = mouseLoc[0];
        y1 = y2 = mouseLoc[1];
        mousePressed = true;
    }

    function handleMouseUp() {
        mousePressed = false;
        updateDiv();
        updateForm();
    }

    function updateDiv() {
        $("#CoordinatesH3")[0].innerText = "(" + x1 + "," + y1 + ") (" + x2 + "," + y2 + ")";
    }

    function mouseOnImageLoc(e) {
        resizedWidth = img.width / canvas[0].offsetWidth;
        var canvasOffset = canvas.offset();
        var offsetX = canvasOffset.left;
        var offsetY = canvasOffset.top - window.scrollY;
        var mouseXLoc = parseInt((e.clientX - offsetX) * resizedWidth);
        var mouseYLoc = parseInt((e.clientY - offsetY) * resizedWidth);
        return [mouseXLoc, mouseYLoc];
    }

    function updateForm() {
        // updates the hidden inputs of the form
        $("#submitInput")[0].disabled = false;
        $("#x1Input")[0].value = x1;
        $("#y1Input")[0].value = y1;
        $("#x2Input")[0].value = x2;
        $("#y2Input")[0].value = y2;
    }
});