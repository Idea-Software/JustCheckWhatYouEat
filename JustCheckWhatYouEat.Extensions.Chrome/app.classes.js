(function ($, undefined) {
    "use strict";

    Idea.VoteRepo = function() {

        if ( Idea.VoteRepo.prototype._singletonInstance ) {
            return Idea.VoteRepo.prototype._singletonInstance;
        }
        Idea.VoteRepo.prototype._singletonInstance = this;

        var buildName = function(cateogry, food, pictureId) {
            return String.format("{0}-{1}-{2}", cateogry, food, pictureId);
        }

        this.Get = function(category, food, pictureId) {
            return localStorage.getItem(buildName(category, food, pictureId));
        };
        this.Has = function(category, food, pictureId) {
            return localStorage.getItem(buildName(category, food, pictureId)) != undefined;
        };
        this.Set = function(category, food, pictureId, vote) {
            localStorage.setItem(buildName(category, food, pictureId), vote);
        };
    };
 

    Idea.FoodPicture = function (url, id, relevance, isUserUploaded, foodInfo) {


        this.Url = url;
        this.Id = id;
        this.Relevance = relevance;
        this.IsUserUploaded = isUserUploaded;
        this.FoodInfo = foodInfo;

        this.$Element = $(Idea.Config.Html.Picture);

        var self = this;

        this.$Element.find("img").on('error', function () {
            self.Log("Picture error");
            self.$Element.remove();
            //todo: show another picture instead ?
            self.Vote("Down");
        });

        this.$Element.find("img").attr("src", this.Url);
        this.$Element.find("img").attr("alt", this.FoodInfo.Food);
        this.$Element.find(".idea_relevance").html(this.Relevance);


        this.$Element.on('click', '.idea_thumbsUp, .idea_thumbsDown', function() {
            self.Vote($(this).attr(Idea.Config.Attr.Vote));
        });

        this.$Element.data({ "obj": this });
    };


    Idea.FoodPicture.prototype = {
        constructor: Idea.FoodPicture,

        Vote: function (vote) {

            this.Log(String.format("Voting {0}", vote));

            var voteRepo = new Idea.VoteRepo();
            var hasVoted = voteRepo.Has(this.FoodInfo.Category, this.FoodInfo.Food, this.Id);
            if (hasVoted) {
                this.Log("User already voted");
                return;
            }

            var self = this;


            $.ajax({
                type: "PATCH",
                url: String.format(Idea.Config.Urls.Picture, encodeURIComponent(self.FoodInfo.Category), encodeURIComponent(self.FoodInfo.Food), self.Id),
                headers: { 'Accept': Idea.Config.Headers.Json, 'Idea.JCWYE.Vote': vote }
            }).done(function (data) {
                self.Log(String.format("Voting {0} - Success", vote));
                voteRepo.Set(self.FoodInfo.Category, self.FoodInfo.Food, self.Id, vote);
                self.Relevance += vote.toLowerCase() === "up" ? 1 : -1;
                self.$Element.find(".idea_relevance").html(self.Relevance);
            });
        },
        Log: function (text) {
            if (Idea.Config.Debug)
                console.log(String.format("FOOD_PICTURE: {0}; Category: {1}, Food: {2}, FoodId: {3}, PictureId: {4}", text, this.FoodInfo.Category, this.FoodInfo.Food, this.FoodInfo.Id, this.Id));
        }
    };

    Idea.FoodInfo = function (category, food, id) {

        this.Category = category;
        this.Food = food;
        this.Id = id;
        this.$Element = $(Idea.Config.Html.Info);
        this.Pictures = [];
        this.Description = "";

        this.DetailsLoaded = false;

        var self = this;

        this.$Element.on("click", function (evt) {
            evt.stopPropagation();
            self.Log("FoodInfo clicked");
            self.OpenDetails();
        });


        this.$Element.data({"obj": this}); //? does it work?
    };


    Idea.FoodInfo.prototype = {
        constructor: Idea.FoodInfo,

        OpenDetails: function () {

            if (!this.DetailsLoaded) {
                this.LoadDetails(true);
                return;
            }

            this.Log(String.format("Opening details with {0} pictures", this.Pictures.length));
            //prepare container
            $("#idea_infoBackdrop, #idea_infoBox").removeClass(Idea.Config.Classes.Hidden);
            $("#idea_infoHead h1").html(this.Food);
            $(Idea.Config.Selectors.Pictures).html("");
            for (var i in this.Pictures.slice(0, 10)) {
                if (!this.Pictures.hasOwnProperty(i))
                    return;
                $(Idea.Config.Selectors.Pictures).append(this.Pictures[i].$Element);
            }

        },
        LoadDetails: function (openOnLoad) {

            this.Log("Loading details");
            var self = this;

            $.ajax({
                type: "GET",
                url: String.format(Idea.Config.Urls.Food, encodeURIComponent(this.Category), encodeURIComponent(this.Food)),
                headers: { Accept: Idea.Config.Headers.Json }
            }).done(function (data) {

                self.Log("Details loaded");
                
                self.Pictures = data.Pictures.map(function (pic, index) {
                    return new Idea.FoodPicture(pic.Url, pic.Id, pic.Relevance, pic.IsUserUploaded, self);
                });

                self.Log(String.format("{0} pictures created", self.Pictures.length));

                self.Description = data.Description;
                self.DetailsLoaded = true;

                if (openOnLoad)
                    self.OpenDetails();
            });
        },
        Log: function (text) {
            console.log(String.format("FOOD_INFO: {0}; Category: {1}, Food: {2}, FoodId: {3}", text, this.Category, this.Food, this.Id));
        }
    };

    Idea.Injector = function () {

        if (Idea.Injector.prototype._singletonInstance)
            return Idea.Injector.prototype._singletonInstance;
        Idea.Injector.prototype._singletonInstance = this;

        var isExcludedFood = function (category, food) {
            var excludedFood = ['special'];
            var regex = new RegExp(excludedFood.join('|'));
            return regex.test(category) || regex.test(food);
        };

        var isInitialized = false;

        this.Initialize = function () {
            if (isInitialized) {
                this.Log("Already initialized.");
                return;
            }
            this.Log("Initializing");
            

            $('head').append(Idea.Config.Html.Styles);
            $('body').prepend(Idea.Config.Html.InfoBox);

            $('.item-name, .itemName').each(function (index, elem) {
                var $this = $(this);

                var food = $this.text().replace(/[\/\\#,+()$~%.'":*?<>{}]/g, '').replace(/[&]/g, 'and').toLowerCase().trim();
                var category = $this.closest(".menu-category").find(".category-header-link").text().replace(/[\/\\#,+()$~%.'":*?<>{}]/g, '').replace(/[&]/g, 'and').toLowerCase().trim();


                if (isExcludedFood(category, food))
                    return;

                var foodInfo = new Idea.FoodInfo(category, food, index);

                $this.prepend(foodInfo.$Element);
            });



            //scroll fix
            $(document).on('mousewheel', Idea.Config.Selectors.Pictures, function (event, delta) {
                $(this)[0].scrollLeft -= (delta * 50);
                event.preventDefault();
            });

            //closing box
            $(document).on('click', '#idea_closeInfo, #idea_infoBackdrop', function () {
                $(Idea.Config.Selectors.Pictures)[0].scrollLeft = 0;
                $("#idea_infoBackdrop, #idea_infoBox").addClass(Idea.Config.Classes.Hidden);
            });


            isInitialized = true;
        },
        this.Log = function (text) {
            console.log(String.format("INJECTOR: {0}; ", text, this.Category, this.Food, this.Id));
        }
    }


}(jQuery));