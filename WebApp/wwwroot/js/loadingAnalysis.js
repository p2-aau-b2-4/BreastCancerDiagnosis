
// lets check if analysis is complete by constantly pinging a site, telling whether its done
window.setInterval(function(){

    $.post( "getStatus", { imageId: FileLoc},function( data ) {
        
        var dataSplitted = data.split(",");
        $( ".result" ).html( dataSplitted[0] );
        $(".progress-bar").css("width",dataSplitted[1]+"%");
        
        if(dataSplitted[0] === ("done")){
            $("#redirect").submit();
        }
    });
    
    
}, 100);
