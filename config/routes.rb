Rails.application.routes.draw do

  resources :charges

  root 'home#index'

  get 'home/index'
  get 'developers'          => 'developers#index'
  get 'contact'             => 'contact#index'
  get 'games'               => 'games#index'
  get 'games/coming_soon'   => 'games#coming_soon'
  get 'play_game/:id'       => 'games#play_game'



end
