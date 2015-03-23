(function ($, undefined) {
    "use strict";

    window.Idea = {
        Config: {
            Html: {
                Picture: '<div class="idea_foodPicture"><div class="idea_relevance"></div><div class="idea_thumbsUp" data-vote="Up" /><div class="idea_thumbsDown" data-vote="Down" /><img src="" alt="" /></div>',
                Info: '<img class="idea_foodInfo" src="//upload.wikimedia.org/wikipedia/commons/thumb/1/1f/Red_information_icon_with_gradient_background.svg/120px-Red_information_icon_with_gradient_background.svg.png" alt="Show Info" title="Show Info"/>',
                InfoBox: '<div id="idea_infoBackdrop" class="hidden"></div><div id="idea_infoBox" class="hidden"><div id="idea_closeInfo"><span class="icon-remove" aria-hidden="true"></span></div><div id="idea_info"><div id="idea_infoHead"><h1>Heading</h1></div><div id="idea_pictures"></div><div id="idea_description"></div></div></div>',
                Styles: '<style> .idea_foodInfo{width: 15px; margin-right:10px; cursor: pointer;}\
			                   .menu-category{overflow: visible;}\
                               .productSynonymListContainer {overflow: visible;}\
                                #idea_infoBackdrop{position: fixed; width: 100%; height: 100%; opacity: .65;   top: 0; left: 0; background-color: #000; z-index: 1001;}\
                                #idea_infoBox{ z-index: 1001; background-color:#fff; width: 500px; height:250px; max-height: 100%; top: 100px; left: 50%; position: fixed; margin-left:-250px;}\
                                #idea_closeInfo{ position: absolute; top: 0;right: 0;color: #fff;font-size: 24px;font-size: 2.4rem;cursor: pointer;height:44px;width: 50px; text-align: right; z-index: 1; }\
                                #idea_closeInfo .icon-remove{ line-height: 1.9; font-size: 22px; font-size: 2.2rem; }\
                                #idea_pictures { width: 500px; overflow:auto; white-space: nowrap;} \
                                #idea_infoHead { display: block; width: 100%; height: 44px; background-color: #ce0b10; margin: 0; padding: 0; position: relative; }\
                                #idea_infoHead h1 {  max-width: 100%; overflow: hidden; text-overflow: ellipsis; margin-left: 35px; margin-right: 35px; white-space: nowrap; color: #fff;   font-size: 1.8rem;   padding-top: 10px;   margin: 0 30px;  font-family: Arial,sans-serif;  text-align: center; font-weight: normal;}\
                                .idea_foodPicture { width:150px; height:100px; overflow:hidden; display:inline-block; position:relative; margin: 2px 1px; }\
                                .idea_foodPicture img { min-width:150px; min-height:100px }\
                                .idea_relevance { position:absolute; background-color:white; opacity:0.8; font-size:14px; width: 20px; height: 20px; top:80px; right:25px; text-align:center; }\
                                .idea_thumbsUp, .idea_thumbsDown {position:absolute; width: 25px; height: 25px; top:75px; right:45px; cursor:pointer;\
                                                                background-image:url("chrome-extension://__MSG_@@extension_id__/images/thumbs.png"); \
                                                                background-image:url("chrome-extension://ljifmlgbhaegafmbcdjamokbdolbbkpe/images/thumbs.png");} \
                                .idea_thumbsDown {right:0px; background-position-x: -25px;}\
                                .hidden{display:none !important;}\
		                        </style>'
            },
            Urls: {
                Picture: "//justcheckwhatyoueat.azurewebsites.net/category/{0}/food/{1}/picture/{2}",
                Food: "//justcheckwhatyoueat.azurewebsites.net/category/{0}/food/{1}"
            },

            Events: {
                DataRecevied: {
                    Food: 'food.dataReceived.Idea',
                    VoteResult: 'vote.dataRecevied.Idea'
                }
            },

            Classes: {
                Hidden: 'hidden'
            },

            Selectors: {
                Pictures: '#idea_pictures',
                Picture: '.idea_foodPicture',
                PictureImg: '.idea_foodPicture img'

            },

            Attr: {
                Vote: 'data-vote'
            },

            Headers: {
                Json: 'application/json'
            },

            Debug: true
        }
    };


}(jQuery));