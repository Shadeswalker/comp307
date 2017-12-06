class GamesController < ApplicationController
  before_action :find_game, only: [:play_game]

  def index
    @games = Game.all
  end

  def play_game
  end

  private
    # Use callbacks to share common setup or constraints between actions.
    def find_game
      @game = Game.find(params[:id])
    end
end
