
// lets check if analysis is complete by constantly pinging a site, telling whether its done
window.setInterval(function(){
    /// call your function here

    $.post( "getStatus", { imageId: FileLoc},function( data ) {
        $( ".result" ).html( data );
    });
    
    
}, 1000);
