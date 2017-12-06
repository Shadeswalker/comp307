class GamesController < ApplicationController
  before_action :find_game, only: [:play_game]

  def index
    @games = Game.all
  end

  def play_game
    if !(@game.fund_goal)
      redirect_to '/games/coming_soon'
    end
  end

  private
    # Use callbacks to share common setup or constraints between actions.
    def find_game
      @game = Game.find(params[:id])
    end
end
