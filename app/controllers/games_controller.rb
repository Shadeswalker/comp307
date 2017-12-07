class GamesController < ApplicationController
  before_action :find_game, only: [:play_game]
  before_action :retrieve_comments, only: [:play_game]

  def index
    @games = Game.all
  end

  def play_game
    if !(@game.fund_goal)
      redirect_to '/games/coming_soon'
    end
  end

  def add_comment
    debugger
    @comment = Comment.new(comment_params)
    @comment.save
    redirect_to "/play_game/#{@comment.game}"
  end

  private
    # Use callbacks to share common setup or constraints between actions.
    def find_game
      @game = Game.find(params[:id])
    end

    def retrieve_comments
      @comments = Comment.where(game: params[:id])
    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def comment_params
      params.require(:comment).permit(:text, :game)
    end
end
