Rails.application.routes.draw do


  root 'home#index'

  get 'home/index'
  get 'developers'        => 'developers#index'
  get 'contact'           => 'contact#index'
  get 'games'             => 'games#index'
  get 'play_game/:id'     => 'games#play_game'



end
