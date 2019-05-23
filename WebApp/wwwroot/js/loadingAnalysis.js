
// lets check if analysis is complete by constantly pinging a site, telling whether its done
window.setInterval(function(){

    $.post( "getStatus", { imageId: FileLoc},function( data ) {
        
        var dataSplitted = data.split(",");
        $( ".result" ).html( dataSplitted[0] );
        $(".progress-bar").css("width",dataSplitted[1]+"%");
        
        if(dataSplitted[1] === ("100")){ //100% done
            $("#redirect").submit();
        }
        if(dataSplitted[0].startsWith("FATAL FEJL")){
            // lets remove the loadinganimation:
            $(".spinner-border").remove();
            $(".result").append("<h3>"+dataSplitted.slice(1,dataSplitted.length-1)+"</h3>")
        }
    });
    
    
}, 500);
